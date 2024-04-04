using krisna_dto.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;

namespace krisna_dto.Data
{
    public class BookData
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;

        public BookData(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        //SelectAll
        public List<Book> GetAll()
        {
            List<Book> books = new List<Book>();
            string query = "SELECT * FROM Books";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
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
                    }

                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                    

                }

            }

            return books;

        }

        //Select By Primary Key
        public Book? GetById(Guid id)
        {
            Book? book = null;

            string query = $"SELECT * FROM Books WHERE Id = @Id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Id", id);

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


        public Book? GetByTitle(string title)
        {
            Book? book = null;

            string query = $"SELECT * FROM Books WHERE Title = @title";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@title", title);

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

			string query = $"INSERT INTO Books(Id, Title, Description, Author, Stock, Created, Updated) " + $"VALUES(@Id, @Title, @Description, @Author, @Stock, @Created, @Updated)";

			// string query = $"INSERT INTO Books(Id, Title, Description, Author, Stock, Created, Updated) " + $"VALUES ('{book.Id}', '{book.Title}', '{book.Description}', '{book.Author}', '{book.Stock}', '{created}' '{updated}')";


			using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
					command.Connection = connection;
					command.CommandText = query;

					command.Parameters.AddWithValue("@Id", book.Id);
					command.Parameters.AddWithValue("@Title", book.Title);
					command.Parameters.AddWithValue("@Description", book.Description);
					command.Parameters.AddWithValue("@Author", book.Author);
					command.Parameters.AddWithValue("@Stock", book.Stock);
					command.Parameters.AddWithValue("@Created", book.Created);
					command.Parameters.AddWithValue("@Updated", book.Updated);

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

			string query = $"UPDATE Books SET Title = @Title, Description = @Description, Author = @Author, Stock = @Stock, Updated = @Updated " + $"WHERE Id = @Id";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@Id", book.Id);
                    command.Parameters.AddWithValue("@Title", book.Id);
                    command.Parameters.AddWithValue("@Description", book.Description);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@Stock", book.Stock);
                    command.Parameters.AddWithValue("@Updated", updated);

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

