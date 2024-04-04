using System;
using krisna_dto.Data;

namespace krisna_dto.Models
{
	public class Book
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
        public string Author { get; set; } = string.Empty;
        public int Stock { get; set; }
		public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}

//private readonly IConfiguration _configuration;
//private readonly string connectionString;

//public BookData(IConfiguration configuration)
//{
//    _configuration = configuration;
//    connectionString = _configuration.GetConnectionString("DefaultConnection");
//}
