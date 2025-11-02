using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace Smart_Home_Project
{
    /// <summary>
    /// Сервіс для взаємодії з локальною базою даних SQLite.
    /// Відповідає за створення структури БД, CRUD операції та логування.
    /// </summary>
    public class DatabaseService
    {
        #region Поля (Fields)

        // Шлях до файлу БД.
        private readonly string dbPath;
        // Рядок підключення до БД.
        private readonly string connectionString;

        #endregion

        #region Конструктор

        public DatabaseService()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory; // Директорія програми.
            dbPath = Path.Combine(baseDir, "smarthome_security.db");
            connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase(); // Ініціалізація БД при створенні сервісу.
        }

        #endregion

        #region Ініціалізація Бази Даних

        /// <summary>
        /// Перевіряє існування файлу БД. Якщо його немає - створює файл та таблиці.
        /// Заповнює таблиці початковими даними при першому створенні БД.
        /// </summary>
        private void InitializeDatabase()
        {
            bool dbExists = File.Exists(dbPath);
            if (!dbExists)
            {
                SQLiteConnection.CreateFile(dbPath); // Створення порожнього файлу БД.
                Debug.WriteLine($"Database file created at: {dbPath}");
            }

            // Встановлення з'єднання та створення таблиць.
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();

            // SQL запити для створення таблиць, якщо вони не існують.
            string createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        PasswordSalt TEXT NOT NULL
                    );";

            string createRoomsTable = @"
                    CREATE TABLE IF NOT EXISTS Rooms (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE
                    );";

            string createDeviceTypesTable = @"
                    CREATE TABLE IF NOT EXISTS DeviceTypes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE
                    );";


            string createDevicesTable = @"
                    CREATE TABLE IF NOT EXISTS Devices (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        RoomId INTEGER NOT NULL,
                        TypeId INTEGER NOT NULL,
                        Status TEXT NOT NULL,
                        FOREIGN KEY (RoomId) REFERENCES Rooms(Id),
                        FOREIGN KEY (TypeId) REFERENCES DeviceTypes(Id)
                    );";

            string createLogsTable = @"
                    CREATE TABLE IF NOT EXISTS SecurityLogs (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                        EventType TEXT NOT NULL,
                        Description TEXT,
                        Username TEXT
                    );";

            // Виконання запитів створення таблиць.
            ExecuteNonQuery(connection, createUsersTable);
            ExecuteNonQuery(connection, createRoomsTable);
            ExecuteNonQuery(connection, createDeviceTypesTable);
            ExecuteNonQuery(connection, createDevicesTable);
            ExecuteNonQuery(connection, createLogsTable);

            // Додавання початкових даних (кімнати, типи пристроїв, пристрої) тільки при першому створенні БД.
            if (!dbExists)
            {
                AddDefaultData(connection);
                Debug.WriteLine("Default data added to the database.");
            }
            // Користувач 'admin' додається з MainForm при першому запуску,
            // щоб уникнути циклічної залежності сервісів.
        }

        /// <summary>
        /// Додає стандартні кімнати, типи пристроїв та пристрої до БД.
        /// </summary>
        private void AddDefaultData(SQLiteConnection connection)
        {
            // Типи пристроїв.
            ExecuteNonQuery(connection, "INSERT OR IGNORE INTO DeviceTypes (Name) VALUES ('Лампа');");
            ExecuteNonQuery(connection, "INSERT OR IGNORE INTO DeviceTypes (Name) VALUES ('Термостат');");
            int lampTypeId = GetIdByName(connection, "DeviceTypes", "Лампа");
            int thermostatTypeId = GetIdByName(connection, "DeviceTypes", "Термостат");


            // Кімнати.
            ExecuteNonQuery(connection, "INSERT OR IGNORE INTO Rooms (Name) VALUES ('Вітальня');");
            ExecuteNonQuery(connection, "INSERT OR IGNORE INTO Rooms (Name) VALUES ('Спальня');");
            ExecuteNonQuery(connection, "INSERT OR IGNORE INTO Rooms (Name) VALUES ('Кухня');");
            int livingRoomId = GetIdByName(connection, "Rooms", "Вітальня");
            int bedroomId = GetIdByName(connection, "Rooms", "Спальня");
            int kitchenId = GetIdByName(connection, "Rooms", "Кухня");

            // Пристрої.
            if (livingRoomId > 0 && lampTypeId > 0 && thermostatTypeId > 0)
            {
                ExecuteNonQuery(connection, $"INSERT INTO Devices (Name, RoomId, TypeId, Status) VALUES ('Основне світло', {livingRoomId}, {lampTypeId}, 'Вимкнено');");
                ExecuteNonQuery(connection, $"INSERT INTO Devices (Name, RoomId, TypeId, Status) VALUES ('Термостат', {livingRoomId}, {thermostatTypeId}, '21°C');");
            }
            if (bedroomId > 0 && lampTypeId > 0)
            {
                ExecuteNonQuery(connection, $"INSERT INTO Devices (Name, RoomId, TypeId, Status) VALUES ('Нічник', {bedroomId}, {lampTypeId}, 'Вимкнено');");
            }
            if (kitchenId > 0 && lampTypeId > 0)
            {
                ExecuteNonQuery(connection, $"INSERT INTO Devices (Name, RoomId, TypeId, Status) VALUES ('Освітлення стільниці', {kitchenId}, {lampTypeId}, 'Вимкнено');");
            }
        }

        /// <summary>
        /// Допоміжний метод для отримання ID запису за його іменем з вказаної таблиці.
        /// </summary>
        private int GetIdByName(SQLiteConnection connection, string tableName, string name)
        {
            string query = $"SELECT Id FROM {tableName} WHERE Name = @Name LIMIT 1;";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);
            var result = command.ExecuteScalar(); // Повертає перший стовпець першого рядка.
                                                  // Перевірка на null та конвертація в int.
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : -1;
        }

        #endregion

        #region Керування користувачами (User Management)

        /// <summary>
        /// Отримує дані користувача з БД за його іменем.
        /// </summary>
        /// <returns>Об'єкт User або null, якщо користувача не знайдено.</returns>
        public User? GetUserByUsername(string username)
        {
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            string query = "SELECT Id, Username, PasswordHash, PasswordSalt FROM Users WHERE Username = @Username LIMIT 1;";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read()) // Якщо знайдено хоча б один рядок.
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    PasswordSalt = reader.GetString(3) // Зчитуємо сіль.
                };
            }
            return null; // Користувача не знайдено.
        }

        /// <summary>
        /// Додає користувача за замовчуванням (викликається з MainForm).
        /// Використовує INSERT OR IGNORE для уникнення дублікатів.
        /// </summary>
        public void AddDefaultUser(string username, string hashString, string saltString)
        {
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            // INSERT OR IGNORE - ігнорує помилку, якщо користувач з таким Username вже існує.
            string query = "INSERT OR IGNORE INTO Users (Username, PasswordHash, PasswordSalt) VALUES (@Username, @PasswordHash, @PasswordSalt);";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@PasswordHash", hashString);
            command.Parameters.AddWithValue("@PasswordSalt", saltString);
            command.ExecuteNonQuery();
        }

        #endregion

        #region Керування кімнатами та пристроями (Room & Device Management)

        /// <summary>
        /// Повертає список всіх кімнат, відсортованих за іменем.
        /// </summary>
        public List<Room> GetRooms()
        {
            var rooms = new List<Room>();
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            string query = "SELECT Id, Name FROM Rooms ORDER BY Name;";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                rooms.Add(new Room { Id = reader.GetInt32(0), Name = reader.GetString(1) });
            }
            return rooms;
        }

        /// <summary>
        /// Повертає список пристроїв у вказаній кімнаті, включаючи назву типу пристрою.
        /// </summary>
        public List<Device> GetDevicesInRoom(int roomId)
        {
            var devices = new List<Device>();
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            // JOIN з таблицею DeviceTypes для отримання назви типу.
            string query = @"
                    SELECT d.Id, d.Name, d.Status, dt.Name AS TypeName
                    FROM Devices d
                    JOIN DeviceTypes dt ON d.TypeId = dt.Id
                    WHERE d.RoomId = @RoomId
                    ORDER BY d.Name;";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@RoomId", roomId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                devices.Add(new Device
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Status = reader.GetString(2),
                    DeviceType = reader.GetString(3) // Назва типу пристрою.
                });
            }
            return devices;
        }

        /// <summary>
        /// Оновлює статус пристрою в БД за його ID.
        /// </summary>
        public void UpdateDeviceStatus(int deviceId, string newStatus)
        {
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            string query = "UPDATE Devices SET Status = @Status WHERE Id = @Id;";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Status", newStatus);
            command.Parameters.AddWithValue("@Id", deviceId);
            int rowsAffected = command.ExecuteNonQuery(); // Повертає кількість змінених рядків.
            Debug.WriteLineIf(rowsAffected > 0, $"Device {deviceId} status updated to '{newStatus}'.");
            Debug.WriteLineIf(rowsAffected == 0, $"Device {deviceId} not found for status update.");
        }


        #endregion

        #region Логування подій безпеки

        /// <summary>
        /// Записує подію безпеки до таблиці SecurityLogs.
        /// </summary>
        public void LogSecurityEvent(string eventType, string description, string? username)
        {
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            string query = "INSERT INTO SecurityLogs (EventType, Description, Username) VALUES (@EventType, @Description, @Username);";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@EventType", eventType);
            command.Parameters.AddWithValue("@Description", description);
            // Обробка можливого null значення для username (напр., системні події).
            command.Parameters.AddWithValue("@Username", (object?)username ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Повертає список останніх 100 подій безпеки, відсортованих за часом.
        /// </summary>
        public List<SecurityLog> GetSecurityLogs()
        {
            var logs = new List<SecurityLog>();
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            // Обмеження LIMIT 100 для запобігання завантаженню великої кількості даних.
            string query = "SELECT Id, Timestamp, EventType, Description, Username FROM SecurityLogs ORDER BY Timestamp DESC LIMIT 100;";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                logs.Add(new SecurityLog
                {
                    Id = reader.GetInt32(0),
                    Timestamp = reader.GetDateTime(1),
                    EventType = reader.GetString(2),
                    // Перевірка на DBNull перед зчитуванням рядкових полів.
                    Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    Username = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                });
            }
            return logs;
        }

        #endregion

        #region Допоміжні методи

        /// <summary>
        /// Виконує SQL-запит, який не повертає результату (INSERT, UPDATE, DELETE, CREATE).
        /// Додано базову обробку помилок SQLite.
        /// </summary>
        private void ExecuteNonQuery(SQLiteConnection connection, string commandText)
        {
            try
            {
                using var command = new SQLiteCommand(commandText, connection);
                command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                // Логування помилки виконання SQL.
                Debug.WriteLine($"SQLite Error executing command: {commandText}\nError: {ex.Message}");
                // В реальному додатку потрібна краща обробка, можливо, сповіщення користувача.
            }
        }

        #endregion
    }
}