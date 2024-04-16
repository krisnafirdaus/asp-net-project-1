using System;
using krisna_dto.Models;
using krisna_dto.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using krisna_dto.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using krisna_dto.Email;
using Microsoft.AspNetCore.WebUtilities;

namespace krisna_dto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

	public class UserController : ControllerBase
	{
        private readonly UserData _userData;
        private readonly IConfiguration _configuration;
        private readonly EmailService _mail;

        public UserController(UserData userData, IConfiguration configuration)
        {
            _userData = userData;
            _configuration = configuration;
        }

        [HttpPost("CreateUser")]

        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDto)
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
                    bool mailResult = await SendEmailActivation(user);
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

        [HttpGet("ActivateUser")]

        public IActionResult ActivateUser(Guid id, string username)
        {
            try
            {
                User? user = _userData.CheckUserAuth(username);

                if (user == null) return BadRequest("Activation Failed");

                if (user.IsActivated == true) return BadRequest("User has been activated");

                bool result = _userData.ActiveUser(id);

                if (result) return Ok("User Activated"); else return StatusCode(500, "Activation Failed");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("ForgetPassword")]

        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) return BadRequest("Email is empty");

                bool sendEmail = await SendEmailForgetPassword(email);

                if (sendEmail)
                {
                    return Ok("Mail Sent");
                }
                else
                {
                    return StatusCode(500, "Error");
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("ResetPassword")]

        public IActionResult ResetPassword([FromBody] ResetPasswordDTO resetPassword)
        {
            try
            {
                if (resetPassword == null) return BadRequest("No Data");

                if(resetPassword.Password != resetPassword.ConfirmPassword)
                {
                    return BadRequest("Password doesn't match");
                }

                bool reset = _userData.ResetPassword(resetPassword.Email, BCrypt.Net.BCrypt.HashPassword(resetPassword.Password));

                if (reset)
                {
                    return Ok("Reset password OK");
                }
                else
                {
                    return StatusCode(500, "Error");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private async Task<bool> SendEmailForgetPassword(string email)
        {
            List<string> to = new List<string>();
            to.Add(email);

            string subject = "Forget Password";

            var param = new Dictionary<string, string?>
            {
                {"username", email}
            };

            string callbackUrl = QueryHelpers.AddQueryString("https://localhost:3000/formResetPassword", param);

            string body = "Please reset your password <a href=\"" + callbackUrl + "\">here</a>";

            EmailModel mailModel = new EmailModel(to, subject, body);

            bool mailResult = await _mail.SendAsync(mailModel, new CancellationToken());

            return mailResult;
        }

        private async Task<bool> SendEmailActivation(User user)
        {
            if (user == null) return false;

            if (string.IsNullOrEmpty(user.Email)) return false;

            List<string> to = new List<string>();
            to.Add(user.Email);

            string subject = "Account Activation";
            var param = new Dictionary<string, string?>
            {
                {"id", user.Id.ToString() },
                {"username", user.Username }
            };

            string callbackUrl = QueryHelpers.AddQueryString("https://localhost:7078/api/User/ActivateUser", param);

            //string body = "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>";

            EmailActivationModel model = new EmailActivationModel()
            {
                Email = user.Email,
                Link = callbackUrl
            };

            string body = _mail.GetEmailTemplate(model);

            EmailModel mailModel = new EmailModel(to, subject, body);
            bool mailResult = await _mail.SendAsync(mailModel, new CancellationToken());
            return mailResult;
        }

        
    }
}

