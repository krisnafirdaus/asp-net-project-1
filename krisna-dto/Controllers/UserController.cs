using System;
using krisna_dto.Models;
using krisna_dto.DTOs.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using krisna_dto.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace krisna_dto.Controllers
{
    [Route("ap/[controller]")]
    [ApiController]

	public class UserController : ControllerBase
	{
        private readonly UserData _userData;
        private readonly IConfiguration _configuration;

        public UserController(UserData userData, IConfiguration configuration)
        {
            _userData = userData;
            _configuration = configuration;
        }

        [HttpPost("CreateUser")]

        public IActionResult CreateUser([FromBody] UserDTO userDto)
        {
            try
            {
                User user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = userDto.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                    Email = userDto.Email,
                    IsActivated = false,
                };

                UserRole userRole = new UserRole
                {
                    UserId = user.Id,
                    Role = userDto.Role
                };

                bool result = _userData.CreateUserAccount(user, userRole);

                if (result)
                {
                    return StatusCode(201, userDto);
                }
                else
                {
                    return StatusCode(500, "Data not inserted");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("login")]

        public IActionResult Login([FromBody] LoginRequestDTO credential)
        {
            if (credential is null) return BadRequest("Invalid client request");

            if(string.IsNullOrEmpty(credential.Username) || string.IsNullOrEmpty(credential.Password)) return BadRequest("Invalid client request");

            User? user = _userData.CheckUserAuth(credential.Username);

            if (user == null) return Unauthorized("You do not authorized");

            if (!user.IsActivated)
            {
                return Unauthorized("Please Active your account");
            }

            UserRole? userRole = _userData.GetUserRole(user.Id);

            bool isVerified = BCrypt.Net.BCrypt.Verify(credential.Password, user?.Password);
            //bool isVerified = user?.Password == credential.Password;

            if(user != null && !isVerified)
            {
                return BadRequest("Inccorrect Password! Please check your password");
            }
            else
            {
                var key = _configuration.GetSection("JwtConfig:Key").Value;
                var JwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, userRole.Role)
                };

                var signingCredential = new SigningCredentials(
                    JwtKey, SecurityAlgorithms.HmacSha256Signature
                );

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = signingCredential
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var securityToken = tokenHandler.CreateToken(tokenDescriptor);

                string token = tokenHandler.WriteToken(securityToken);

                return Ok(new LoginResponseDTO { Token = token });

            }
        }
    }
}

