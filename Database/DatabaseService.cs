﻿using Cine_Critic_AI.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Cine_Critic_AI.Services
{
    public sealed class DatabaseService
    {
        private static readonly Lazy<DatabaseService> lazy =
            new Lazy<DatabaseService>(() => new DatabaseService());

        public static DatabaseService Instance => lazy.Value;

        private readonly string _connectionString;

        private DatabaseService()
        {
            _connectionString = "Data Source=CineCriticDB.sqlite";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                Email TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL,
                RegisteredOn TEXT
            );

            CREATE TABLE IF NOT EXISTS Movies(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Year INTEGER NOT NULL,
                Genre TEXT NOT NULL,
                Director TEXT NOT NULL,
                Description TEXT,
                ImageUrl TEXT
            );

            CREATE TABLE IF NOT EXISTS Reviews(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Rate INTEGER NOT NULL,
                Comment TEXT,
                EmotionTone TEXT,
                Date TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS ChatMessages(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Sender TEXT NOT NULL,   -- 'User' или 'Bot'
                Message TEXT NOT NULL,
                Timestamp TEXT NOT NULL DEFAULT (datetime('now'))
            );
            ";
            cmd.ExecuteNonQuery();

            // Проверка и добавяне на MovieId в Reviews, ако липсва
            cmd.CommandText = "PRAGMA table_info(Reviews);";
            using var reader = cmd.ExecuteReader();
            bool movieIdExists = false;
            while (reader.Read())
            {
                if (reader["name"].ToString() == "MovieId")
                {
                    movieIdExists = true;
                    break;
                }
            }
            reader.Close();

            if (!movieIdExists)
            {
                cmd.CommandText = "ALTER TABLE Reviews ADD COLUMN MovieId INTEGER DEFAULT 1;";
                cmd.ExecuteNonQuery();
            }

            // Проверка и добавяне на ImageUrl в Movies, ако липсва
            cmd.CommandText = "PRAGMA table_info(Movies);";
            using var reader2 = cmd.ExecuteReader();
            bool imageUrlExists = false;
            while (reader2.Read())
            {
                if (reader2["name"].ToString() == "ImageUrl")
                {
                    imageUrlExists = true;
                    break;
                }
            }
            reader2.Close();

            if (!imageUrlExists)
            {
                cmd.CommandText = "ALTER TABLE Movies ADD COLUMN ImageUrl TEXT;";
                cmd.ExecuteNonQuery();
            }
        }

        // ================== USERS ==================
        public void InsertUser(User user)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Users (Username, Email, Password, RegisteredOn)
                VALUES (@Username, @Email, @Password, @RegisteredOn)";
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@RegisteredOn", user.RegisteredOn.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
        }

        public void UpdateUser(User user)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Users
                SET Username = @Username,
                    Email = @Email,
                    Password = @Password
                WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@Id", user.Id);
            cmd.ExecuteNonQuery();
        }

        public User GetUserByUsername(string username)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users WHERE Username = @Username";
            cmd.Parameters.AddWithValue("@Username", username);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"].ToString(),
                    Email = reader["Email"].ToString(),
                    Password = reader["Password"].ToString(),
                    RegisteredOn = DateTime.Parse(reader["RegisteredOn"].ToString())
                };
            }
            return null;
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"].ToString(),
                    Email = reader["Email"].ToString(),
                    Password = reader["Password"].ToString(),
                    RegisteredOn = DateTime.Parse(reader["RegisteredOn"].ToString())
                });
            }
            return users;
        }

        // ================== MOVIES ==================
        public void InsertMovie(Movie movie)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Movies (Title, Year, Genre, Director, Description, ImageUrl)
                VALUES (@Title, @Year, @Genre, @Director, @Description, @ImageUrl)";
            cmd.Parameters.AddWithValue("@Title", movie.Title);
            cmd.Parameters.AddWithValue("@Year", movie.Year);
            cmd.Parameters.AddWithValue("@Genre", movie.Genre);
            cmd.Parameters.AddWithValue("@Director", movie.Director);
            cmd.Parameters.AddWithValue("@Description", movie.Description ?? "");
            cmd.Parameters.AddWithValue("@ImageUrl", movie.ImageUrl ?? "");
            cmd.ExecuteNonQuery();
        }

        public void UpdateMovie(Movie movie)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Movies
                SET Title = @Title,
                    Year = @Year,
                    Genre = @Genre,
                    Director = @Director,
                    Description = @Description,
                    ImageUrl = @ImageUrl
                WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Title", movie.Title);
            cmd.Parameters.AddWithValue("@Year", movie.Year);
            cmd.Parameters.AddWithValue("@Genre", movie.Genre);
            cmd.Parameters.AddWithValue("@Director", movie.Director);
            cmd.Parameters.AddWithValue("@Description", movie.Description ?? "");
            cmd.Parameters.AddWithValue("@ImageUrl", movie.ImageUrl ?? "");
            cmd.Parameters.AddWithValue("@Id", movie.Id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteMovie(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Movies WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Movie> GetAllMovies()
        {
            var movies = new List<Movie>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Movies";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                movies.Add(new Movie
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Title = reader["Title"].ToString(),
                    Year = Convert.ToInt32(reader["Year"]),
                    Genre = reader["Genre"].ToString(),
                    Director = reader["Director"].ToString(),
                    Description = reader["Description"].ToString(),
                    ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : ""
                });
            }
            return movies;
        }

        public Movie GetMovieById(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Movies WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Movie
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Title = reader["Title"].ToString(),
                    Year = Convert.ToInt32(reader["Year"]),
                    Genre = reader["Genre"].ToString(),
                    Director = reader["Director"].ToString(),
                    Description = reader["Description"].ToString(),
                    ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : ""
                };
            }
            return null;
        }

        // ================== REVIEWS ==================
        public void InsertReview(Review review)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Reviews (Rate, Comment, EmotionTone, Date, MovieId)
                VALUES (@Rate, @Comment, @EmotionTone, @Date, @MovieId)";
            cmd.Parameters.AddWithValue("@Rate", review.Rate);
            cmd.Parameters.AddWithValue("@Comment", review.Comment ?? "");
            cmd.Parameters.AddWithValue("@EmotionTone", review.EmotionTone ?? "");
            cmd.Parameters.AddWithValue("@Date", review.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@MovieId", review.MovieId);
            cmd.ExecuteNonQuery();
        }

        public void UpdateReview(Review review)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Reviews
                SET Rate = @Rate,
                    Comment = @Comment,
                    EmotionTone = @EmotionTone,
                    Date = @Date
                WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Rate", review.Rate);
            cmd.Parameters.AddWithValue("@Comment", review.Comment ?? "");
            cmd.Parameters.AddWithValue("@EmotionTone", review.EmotionTone ?? "");
            cmd.Parameters.AddWithValue("@Date", review.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@Id", review.Id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteReview(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Reviews WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Review> GetAllReviews()
        {
            var reviews = new List<Review>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT r.*, m.Title, m.Year, m.Genre, m.Director, m.Description, m.ImageUrl
                FROM Reviews r
                JOIN Movies m ON r.MovieId = m.Id";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                reviews.Add(new Review
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Rate = Convert.ToInt32(reader["Rate"]),
                    Comment = reader["Comment"].ToString(),
                    EmotionTone = reader["EmotionTone"].ToString(),
                    Date = DateTime.Parse(reader["Date"].ToString()),
                    MovieId = Convert.ToInt32(reader["MovieId"]),
                    Movie = new Movie
                    {
                        Id = Convert.ToInt32(reader["MovieId"]),
                        Title = reader["Title"].ToString(),
                        Year = Convert.ToInt32(reader["Year"]),
                        Genre = reader["Genre"].ToString(),
                        Director = reader["Director"].ToString(),
                        Description = reader["Description"].ToString(),
                        ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : ""
                    }
                });
            }
            return reviews;
        }

        public Review GetReviewById(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Reviews WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Review
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Rate = Convert.ToInt32(reader["Rate"]),
                    Comment = reader["Comment"].ToString(),
                    EmotionTone = reader["EmotionTone"].ToString(),
                    Date = DateTime.Parse(reader["Date"].ToString()),
                    MovieId = reader["MovieId"] != DBNull.Value ? Convert.ToInt32(reader["MovieId"]) : 0
                };
            }
            return null;
        }

        // ================== CHAT MESSAGES INSERT ==================
        public void InsertChatMessage(ChatMessage msg)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
        INSERT INTO ChatMessages (UserId, Sender, Message, Timestamp)
        VALUES (@UserId, @Sender, @Message, @Timestamp)";
            cmd.Parameters.AddWithValue("@UserId", msg.UserId);
            cmd.Parameters.AddWithValue("@Sender", msg.Sender);
            cmd.Parameters.AddWithValue("@Message", msg.Message);
            cmd.Parameters.AddWithValue("@Timestamp", msg.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
        }


        // ================== CHAT MESSAGES GET ==================
        public List<ChatMessage> GetChatMessagesByUser(int userId)
        {
            var messages = new List<ChatMessage>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM ChatMessages WHERE UserId = @UserId ORDER BY Timestamp";
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                messages.Add(new ChatMessage
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    UserId = Convert.ToInt32(reader["UserId"]),
                    Sender = reader["Sender"].ToString(),
                    Message = reader["Message"].ToString(),
                    Timestamp = DateTime.Parse(reader["Timestamp"].ToString())
                });
            }
            return messages;
        }

        // ================== CHAT MESSAGES DELETE ==================
        public void ClearChatByUser(int userId)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM ChatMessages WHERE UserId = @UserId";
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.ExecuteNonQuery();
        }



    }
}
