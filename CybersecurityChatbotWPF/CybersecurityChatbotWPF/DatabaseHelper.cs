// DatabaseHelper.cs - C# 7.3 Compatible
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace CybersecurityChatbotWPF
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }

        public string DisplayText => $"{Title} - {(IsCompleted ? "Completed" : "Pending")}" +
                                      (ReminderDate.HasValue ? $" (Reminder: {ReminderDate.Value:yyyy-MM-dd})" : "");
    }

    public class DatabaseHelper
    {
        private string _connectionString;

        public DatabaseHelper()
        {
            // Get the database path in the application directory
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(appDirectory, "CybersecurityTasks.db");
            _connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Tasks (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Title TEXT NOT NULL,
                            Description TEXT,
                            ReminderDate TEXT,
                            IsCompleted INTEGER DEFAULT 0,
                            CreatedDate TEXT NOT NULL
                        )";

                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
            }
        }

        public int AddTask(string title, string description, DateTime? reminderDate)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string insertQuery = @"
                        INSERT INTO Tasks (Title, Description, ReminderDate, IsCompleted, CreatedDate)
                        VALUES (@Title, @Description, @ReminderDate, @IsCompleted, @CreatedDate);
                        SELECT last_insert_rowid();";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Title", title);
                        command.Parameters.AddWithValue("@Description", description ?? "");
                        command.Parameters.AddWithValue("@ReminderDate", reminderDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "");
                        command.Parameters.AddWithValue("@IsCompleted", 0);
                        command.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding task: {ex.Message}");
                return -1;
            }
        }

        public List<TaskItem> GetTasks(bool includeCompleted = false)
        {
            var tasks = new List<TaskItem>();

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT Id, Title, Description, ReminderDate, IsCompleted, CreatedDate FROM Tasks";
                    if (!includeCompleted)
                    {
                        query += " WHERE IsCompleted = 0";
                    }
                    query += " ORDER BY CreatedDate DESC";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var task = new TaskItem
                                {
                                    Id = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    ReminderDate = reader.IsDBNull(3) ? (DateTime?)null : DateTime.Parse(reader.GetString(3)),
                                    IsCompleted = reader.GetInt32(4) == 1,
                                    CreatedDate = DateTime.Parse(reader.GetString(5))
                                };
                                tasks.Add(task);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting tasks: {ex.Message}");
            }

            return tasks;
        }

        public bool UpdateTaskStatus(int taskId, bool isCompleted)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string updateQuery = "UPDATE Tasks SET IsCompleted = @IsCompleted WHERE Id = @Id";
                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IsCompleted", isCompleted ? 1 : 0);
                        command.Parameters.AddWithValue("@Id", taskId);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating task: {ex.Message}");
                return false;
            }
        }

        public bool DeleteTask(int taskId)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM Tasks WHERE Id = @Id";
                    using (var command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", taskId);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting task: {ex.Message}");
                return false;
            }
        }

        public int GetTaskCount()
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM Tasks WHERE IsCompleted = 0";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}