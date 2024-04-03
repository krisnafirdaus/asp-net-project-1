using krisna_dto.Data;
using krisna_dto.Models;
using krisna_dto.DTOs.Book;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;

namespace krisna_dto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BookController : ControllerBase
    {
        private readonly BookData _bookData;

        public BookController(BookData bookData)
        {
            _bookData = bookData;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            List<Book> books = _bookData.GetAll();
            return Ok(books);
        }

        [HttpGet("GetById")]
        public IActionResult Get(Guid id)
        {
            Book? book = _bookData.GetById(id);

            if (book == null)
            {
                return NotFound("Data Not Found");
            }

            return Ok(book);
        }

        [HttpPost]
        public IActionResult Post([FromBody] BookDTO bookDto)
        {

            if (bookDto == null)
                return BadRequest("Data Should be Inputed");

            Book book = new Book
            {
                Id = Guid.NewGuid(),
                Title = bookDto.Title,
                Description = bookDto.Description,
                Author = bookDto.Title,
                Stock = bookDto.Stock,
                Created = DateTime.Now,
                Updated = DateTime.Now,
            };

            bool result = _bookData.Insert(book);

            if (result)
            {
                return StatusCode(201, book.Id);
            }
            else
            {
                return StatusCode(500, "error occur");
            }

        }

        [HttpPut]
        public IActionResult Put(Guid id, [FromBody] BookDTO bookDto)
        {

            if (bookDto == null)
                return BadRequest("Data Should be Inputed");

            Book book = new Book
            {
                Id = Guid.NewGuid(),
                Title = bookDto.Title,
                Description = bookDto.Description,
                Author = bookDto.Title,
                Stock = bookDto.Stock,
                Updated = DateTime.Now,
            };

            bool result = _bookData.Update(id, book);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, "error occur");
            }

        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            bool result = _bookData.Delete(id);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, "error occur");
            }
        }
    }

}

