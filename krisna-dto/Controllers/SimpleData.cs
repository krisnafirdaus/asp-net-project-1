using krisna_dto.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace krisna_dto.Controllers
{
    [Route("api/[controller]")]
    public class SimpleData : ControllerBase
    {
        [HttpGet("GetData")]
        public IActionResult GetData()
        {
            var result = new SimpleDataInputDTO
            {
                Name = "krisna",
                Date = "10-20-2024"
            };

            return Ok(result);
        }

        [HttpPost("PostData")]
        public IActionResult PostData([FromBody] SimpleDataInputDTO simpleDataInput)
        {
            var result = new SimpleDataInputDTO
            {
                Name = simpleDataInput.Name,
                Date = simpleDataInput.Date
            };

            return Ok(result);
        }
    }
}
