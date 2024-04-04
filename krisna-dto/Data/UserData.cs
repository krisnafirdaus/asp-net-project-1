using System;
using MySql.Data.MySqlClient;
UserRio

namespace krisna_dto.Data
{
	public class UserData
	{
		private readonly string _connectionString;
		private readonly IConfiguration _configuration;

		public UserData(IConfiguration configuration)
		{
			_configuration = configuration;
			_connectionString = _configuration.GetConnectionString("DefaultConnection");
		}

		public bool CreateUserAccount(UserData user, UserRole userRole)
		{
			bool result = false;

			using (MySqlConnection connection = new MySqlConnection(_connectionString))
			{
				using (MySqlCommand command = new MySqlCommand())
				{
					command.Connection = connection;

				}
			}

             return result;
		}
	}
}

