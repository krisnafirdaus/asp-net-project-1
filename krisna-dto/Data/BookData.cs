using krisna_dto.Models;
using MySql.Data.MySqlClient;

namespace krisna_dto.Data
{
	public class BookData
	{
		private readonly string connectionString = "server=localhost;port=3306;database=bookdata;user=root;password=krisna121";

		//SelectAll
		public List<Book> GetAll()
		{
            List<Book> books = new List<Book>();
			string query = "SELECT * FROM Books";
			using(MySqlConnection connection = new MySqlConnection(connectionString))
			{
                using (MySqlCommand command = new MySqlCommand(query, connection))
				{ 
                    connection.Open();

					using (MySqlDataReader reader = command.ExecuteReader())
					{
						while(reader.Read())
						{
							books.Add(new Book
							{
								Id = Guid.Parse(reader["Id"].ToString() ?? string.Empty),
								Title = reader["Title"].ToString() ?? string.Empty,
								Description = reader["Description"].ToString(),
								Author = reader["Author"].ToString() ?? string.Empty,
								Stock = Convert.ToInt32(reader["Stock"]),
								Created = Convert.ToDateTime(reader["Created"]),
								Updated = Convert.ToDateTime(reader["Updated"]),
							});
						}
					}

					connection.Close();

                }

            }

			return books;
		
        }

		//Select By Primary Key
		public Book? GetById(Guid id)
		{
			Book? book = null;

			string query = $"SELECT * FROM Books WHERE Id = '{id}'";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {

					connection.Open();


					using (MySqlDataReader reader = command.ExecuteReader())
					
                    {
						while (reader.Read()) 
						{
							book = new Book
							{
								Id = Guid.Parse(reader["Id"].ToString() ?? string.Empty),
								Title = reader["Title"].ToString() ?? string.Empty,
								Description = reader["Description"].ToString(),
								Author = reader["Author"].ToString() ?? string.Empty,
								Stock = Convert.ToInt32(reader["Stock"]),
								Created = Convert.ToDateTime(reader["Created"]),
								Updated = Convert.ToDateTime(reader["Updated"]),
							};
                        }
                    }
                }

                connection.Close();
            }

			return book;
        }

		//Insert
		public bool Insert(Book book)
		{
			bool result = false;

			string created = book.Created.Date.ToString("yyyy-MM-dd HH:mm:ss");
			string updated = book.Updated.Date.ToString("yyyy-MM-dd HH:mm:ss");

            string query = $"INSERT INTO Books(Id, Title, Description, Author, Stock, Created, Updated) " + $"VALUES ('{book.Id}', '{book.Title}', '{book.Author}', '{book.Stock}', '{created}' '{updated}')";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
					command.Connection = connection;
					command.CommandText = query;

					connection.Open();

					result = command.ExecuteNonQuery() > 0 ? true : false;

					connection.Close();
                }
            }

			return result;

        }

        //Update
		public bool Update(Guid id, Book book)
		{
			bool result = false;

			string updated = book.Updated.Date.ToString("yyyy-MM-dd HH:mm:ss");

			string query = $"UPDATE Books SET Title = '{book.Title}', Description = '{book.Description}', Author = '{book.Author}', Stock = '{book.Stock}', Updated = '{updated} " + $"WHERE Id = '{id}'";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = query;

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }

            return result;
        }

        // Delete
		public bool Delete(Guid id)
		{
			bool result = false;

			string query = $"DELETE FROM Books WHERE Id = '{id}'";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = query;

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }

			return result;
        }
    }
}

