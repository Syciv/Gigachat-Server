using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace GigachatServer.Services
{
    public class DataBaseObject
    {
        NpgsqlConnectionStringBuilder stringBuilder;
        string cs; 

        public DataBaseObject()
        {
            stringBuilder = new NpgsqlConnectionStringBuilder();
            //
            stringBuilder.Host = "localhost";
            stringBuilder.Database = "Gigachat";
            stringBuilder.Port = 5432;
            //
            stringBuilder.Password = Environment.GetEnvironmentVariable("PgPassword");
            stringBuilder.Username = Environment.GetEnvironmentVariable("PgLogin");

            cs = stringBuilder.ToString();
        } 

        // Добавление нового пользователя
        public string AddUser(string userName, byte[] passwordHash, byte[] salt)
        {
            using (var connection = new NpgsqlConnection(cs))
            {
                connection.Open();

                var query = @"INSERT INTO Users (Login, PasswordHash, Salt) values (quote_ident(@name), @passwordHash, @salt)";

                var command = new NpgsqlCommand();
                command.CommandText = query;
                command.Connection = connection;
                command.Parameters.AddWithValue("@name", userName);
                command.Parameters.AddWithValue("@passwordHash", passwordHash);
                command.Parameters.AddWithValue("@salt", salt);

                if (command.ExecuteNonQuery() == 1)
                {
                    return "Пользователь добавлен";
                }
                else
                {
                    return "Произошла ошибка";
                }
            }
        }
    }
}
