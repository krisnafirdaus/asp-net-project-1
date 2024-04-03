using System;
namespace krisna_dto.DTOs.Book
{
	public class BookDTO
	{
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string Author { get; set; } = string.Empty;
		public int Stock { get; set; }
    }
}

