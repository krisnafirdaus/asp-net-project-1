using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace krisna_dto.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpGet("GetGuest")]
        [Authorize(Roles = "guest")]
        public IActionResult GetGuest()
        {
            return Ok("This is guest");
        }

        [HttpGet("GetAdmin")]
        [Authorize(Roles = "admin")]
        public IActionResult GetAdmin()
        {
            return Ok("This is admin");
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "admin, guest")]
        public IActionResult GetAll()
        {
            return Ok("This is guest & Admin");
        }
    }
}

