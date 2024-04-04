using System;
namespace krisna_dto.Models
{
	public class UserRole
	{
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string Role { get; set; } = string.Empty;
    }
}

