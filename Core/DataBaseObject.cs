using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using GigachatServer.Services;
using GigachatServer.Models;

namespace GigachatServer.Core
{
    public class DataBaseObject
    {
        NpgsqlConnectionStringBuilder stringBuilder;
        string cs;

        public DataBaseObject()
        {
            stringBuilder = new NpgsqlConnectionStringBuilder();
            // <Перенести в файл>
            stringBuilder.Host = "localhost";
            stringBuilder.Database = "Gigachat";
            stringBuilder.Port = 5432;
            //
            stringBuilder.Password = Environment.GetEnvironmentVariable("PgPassword");
            stringBuilder.Username = Environment.GetEnvironmentVariable("PgLogin");

            cs = stringBuilder.ToString();
        }

        // Добавление нового пользователя
        public (int, string) AddUser(string userName, string password, string name, string surname)
        {
            using (var connection = new NpgsqlConnection(cs))
            {
                connection.Open();

                // Проверка, существует ли пользователь с таким логином

                var checkQuery = @"SELECT * FROM Users WHERE Login = quote_ident(@log)";
                var command = new NpgsqlCommand();
                command.CommandText = checkQuery;
                command.Connection = connection;
                command.Parameters.AddWithValue("@log", userName);

                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    reader.Close();
                    return (0, "Логин занят");
                }
                else
                {
                    reader.Close();
                    // Добавляем пользователя в БД, если такого ещё нет

                    var addQuery = @"INSERT INTO Users (Login, PasswordHash, Salt, Name, Surname) values (quote_ident(@log), @passwordHash, @salt, quote_ident(@name), quote_ident(@surname))";

                    byte[] salt = HashService.GetSalt();
                    byte[] passwordHash = HashService.GetHash(password + salt);

                    command.CommandText = addQuery;
                    command.Connection = connection;
                    command.Parameters.AddWithValue("@log", userName);
                    command.Parameters.AddWithValue("@passwordHash", passwordHash);
                    command.Parameters.AddWithValue("@salt", salt);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@surname", surname);

                    if (command.ExecuteNonQuery() == 1)
                    {
                        return (1, "Пользователь добавлен");
                    }
                    else
                    {
                        return (0, "Произошла ошибка");
                    }
                }
            }
        }

        public (int, string, byte[]) GetSalt(string userName)
        {
            using (var connection = new NpgsqlConnection(cs))
            {
                connection.Open();

                // Проверка, существует ли пользователь с таким логином

                byte[] salt;
                var checkQuery = @"SELECT Salt FROM Users WHERE Login = quote_ident(@log)";
                var command = new NpgsqlCommand();
                command.CommandText = checkQuery;
                command.Connection = connection;
                command.Parameters.AddWithValue("@log", userName);

                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    salt = (byte[])reader["Salt"];
                    reader.Close();
                    return (1, "Пользователь найден", salt);
                }
                else
                {
                    reader.Close();
                    salt = null;
                    return (0, "Нет такого пользователя!", salt);
                }
            }
        }


        public (int, string) Auntheficate(string userName, string password)
        {
            using (var connection = new NpgsqlConnection(cs))
            {
                connection.Open();

                (int result, string message, byte[] salt) = GetSalt(userName);

                if (result == 1)
                {
                    var checkQuery = @"SELECT * FROM Users WHERE Login = quote_ident(@log) AND PasswordHash = @hash AND Salt = @salt";
                    var command = new NpgsqlCommand();
                    command.CommandText = checkQuery;
                    command.Connection = connection;

                    byte[] passwordHash = HashService.GetHash(password + salt);

                    command.Parameters.AddWithValue("@log", userName);
                    command.Parameters.AddWithValue("@hash", passwordHash);
                    command.Parameters.AddWithValue("@salt", salt); // ???

                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        reader.Close();
                        return (1, "Всё хорошо, залазейте");

                    }
                    else
                    {
                        reader.Close();
                        return (0, "Пароль не правильный");
                    }
                }

                else
                {
                    return (result, message);
                }


            }
        }

        public (int, string, UserProfile) GetUserProfile(string userName)
        {
            using (var connection = new NpgsqlConnection(cs))
            {
                connection.Open();


                var query = @"SELECT login, name, surname, registrationdate, profileimage FROM Users WHERE Login = quote_ident(@log)";
                var command = new NpgsqlCommand();
                command.CommandText = query;
                command.Connection = connection;

                command.Parameters.AddWithValue("@log", userName);

                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    UserProfile userProfile = new UserProfile
                    {
                        UserName = (string)reader["login"],
                        Name = (string)reader["name"],
                        Surname = (string)reader["surname"],
                        Date = ((DateTime)reader["registrationdate"]).ToString(),
                        ProfileImage = (reader["profileimage"] != DBNull.Value) ? (byte[])reader["profileimage"] : null,
                    };

                    reader.Close();
                    return (1, "Распишитесь", userProfile);
                }

                else
                {
                    reader.Close();
                    return (0, "Не получилось.......", null);
                }
            }

        }
    }


}
