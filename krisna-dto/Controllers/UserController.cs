using System;
using krisna_dto.Models;
using krisna_dto.DTOs.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using krisna_dto.Data;

namespace krisna_dto.Controllers
{
    [Route("ap/[controller]")]
    [ApiController]

	public class UserController : ControllerBase
	{
        private readonly UserData _userData;

        public UserController(UserData userData)
        {
            _userData = userData;
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
                    Password = userDto.Password,
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

    }
}

