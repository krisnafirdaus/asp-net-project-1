using System;
using krisna_dto.Models;
using MySql.Data.MySqlClient;

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

        // Single Sql Command
        //public bool CreateUserAccount(User user, UserRole userRole)
        //{
        //	bool result = false;

        //	using (MySqlConnection connection = new MySqlConnection(_connectionString))
        //	{
        //		using (MySqlCommand command = new MySqlCommand())
        //		{
        //			command.Connection = connection;
        //			command.Parameters.Clear();

        //			command.CommandText = "INSERT INTO Users (Id, Username, Password) VALUES (@id, @username, @password)";

        //			command.Parameters.AddWithValue("@id", user.Id);
        //                  command.Parameters.AddWithValue("@username", user.Username);
        //                  command.Parameters.AddWithValue("@password", user.Password);

        //			command.CommandText = "INSERT INTO UserRoles (UserId, Role) VALUES (@userId, @role)";

        //			command.Parameters.AddWithValue("@userId", userRole.UserId);
        //			command.Parameters.AddWithValue("@role", userRole.Role);

        //                  try
        //                  {
        //                      connection.Open();

        //                      int execResult = command.ExecuteNonQuery();

        //                      result = execResult > 0 ? true : false;
        //                  }
        //                  catch
        //                  {
        //                      throw;
        //                  }
        //              }

        //	}

        //           return result;
        //}

        // multiple sql command without transaction
        //public bool CreateUserAccount(User user, UserRole userRole)
        //{

        //    bool result = false;

        //    using (MySqlConnection connection = new MySqlConnection(_connectionString))
        //    {
        //        MySqlCommand command1 = new MySqlCommand();
        //        command1.Connection = connection;
        //        command1.Parameters.Clear();

        //        command1.CommandText = "INSERT INTO Users (Id, Username, Password) VALUES (@id, @username, @password)";
        //        command1.Parameters.AddWithValue("@id", user.Id);
        //        command1.Parameters.AddWithValue("@username", user.Username);
        //        command1.Parameters.AddWithValue("@password", user.Password);

        //        MySqlCommand command2 = new MySqlCommand();
        //        command2.Connection = connection;
        //        command2.Parameters.Clear();


        //        command2.CommandText = "INSERT INTO UserRoles (UserId, Role) VALUES (@userId, @role)";
        //        command2.Parameters.AddWithValue("@userId", userRole.UserId);
        //        command2.Parameters.AddWithValue("@role", userRole.Role);

        //        try
        //        {
        //            connection.Open();

        //            var result1 = command1.ExecuteNonQuery();
        //            var result2 = command2.ExecuteNonQuery();

        //            if (result1 > 0 && result2 > 0) result = true;
        //        }
        //        catch
        //        {
        //            throw;
        //        }
        //    }

        //    return result;

        //}

        // multiple sql command (with transaction)
        public bool CreateUserAccount(User user, UserRole userRole)
        {
            bool result = false;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    MySqlCommand command1 = new MySqlCommand();
                    command1.Connection = connection;
                    command1.Transaction = transaction;
                    command1.Parameters.Clear();

                    command1.CommandText = "INSERT INTO Users (Id, Username, Password) VALUES (@id, @username, @password)";
                    command1.Parameters.AddWithValue("@id", user.Id);
                    command1.Parameters.AddWithValue("@username", user.Username);
                    command1.Parameters.AddWithValue("@password", user.Password);

                    MySqlCommand command2 = new MySqlCommand();
                    command2.Connection = connection;
                    command1.Transaction = transaction;
                    command2.Parameters.Clear();


                    command2.CommandText = "INSERT INTO UserRol (UserId, Role) VALUES (@userId, @role)";
                    command2.Parameters.AddWithValue("@userId", userRole.UserId);
                    command2.Parameters.AddWithValue("@role", userRole.Role);

                    var result1 = command1.ExecuteNonQuery();
                    var result2 = command2.ExecuteNonQuery();

                    transaction.Commit();

                    result = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;

        }
    }
}

