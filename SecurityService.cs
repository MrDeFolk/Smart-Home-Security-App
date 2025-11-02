using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography; // Для Argon2

namespace Smart_Home_Project
{
    /// <summary>
    /// Сервіс, що відповідає за операції безпеки, пов'язані з паролями:
    /// хешування (Argon2id) та валідацію.
    /// </summary>
    public class SecurityService
    {
        #region Залежності (Dependencies)

        // Потрібен для доступу до хешів та солей користувачів у БД.
        private readonly DatabaseService dbService;

        #endregion

        #region Конструктор

        public SecurityService(DatabaseService databaseService)
        {
            dbService = databaseService;
        }

        #endregion

        #region Хешування паролів (Argon2id)

        /// <summary>
        /// Генерує хеш пароля та сіль за допомогою Argon2id.
        /// </summary>
        /// <param name="password">Пароль у відкритому вигляді.</param>
        /// <returns>Кортеж, що містить байтовий масив хешу та байтовий масив солі.</returns>
        public (byte[] hash, byte[] salt) HashPassword(string password)
        {
            // 1. Генерація криптографічно стійкої солі.
            var salt = GenerateSalt();

            // 2. Налаштування параметрів Argon2id.
            // Ці параметри впливають на стійкість до brute-force атак та час виконання.
            // Рекомендовані значення можуть змінюватись з часом.
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8, // Кількість потоків для використання.
                MemorySize = 65536,    // Обсяг пам'яті в кілобайтах (64MB).
                Iterations = 4         // Кількість проходів.
            };

            // 3. Обчислення хешу (32 байти = 256 біт).
            var hash = argon2.GetBytes(32);

            return (hash, salt);
        }

        /// <summary>
        /// Генерує випадкову сіль довжиною 16 байт.
        /// </summary>
        private byte[] GenerateSalt(int size = 16)
        {
            var salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt); // Заповнення масиву випадковими байтами.
            }
            return salt;
        }

        #endregion

        #region Валідація паролів

        /// <summary>
        /// Перевіряє, чи відповідає введений пароль збереженому хешу та солі.
        /// </summary>
        /// <param name="username">Ім'я користувача для пошуку хешу/солі в БД.</param>
        /// <param name="passwordToCheck">Пароль, який потрібно перевірити.</param>
        /// <returns>True, якщо пароль вірний, інакше False.</returns>
        public bool ValidatePassword(string username, string passwordToCheck)
        {
            // 1. Отримання даних користувача (включаючи хеш та сіль) з БД.
            var user = dbService.GetUserByUsername(username);
            if (user == null)
            {
                Debug.WriteLine($"ValidatePassword: User '{username}' not found.");
                return false; // Користувача не існує.
            }

            try
            {
                // 2. Конвертація збережених Base64 рядків назад у байти.
                var storedHash = Convert.FromBase64String(user.PasswordHash);
                var storedSalt = Convert.FromBase64String(user.PasswordSalt); // Використовуємо правильну властивість.

                // 3. Обчислення хешу для введеного пароля з використанням збереженої солі.
                var argon2 = new Argon2id(Encoding.UTF8.GetBytes(passwordToCheck))
                {
                    Salt = storedSalt, // Важливо: використовуємо ту ж сіль!
                    DegreeOfParallelism = 8,
                    MemorySize = 65536,
                    Iterations = 4
                };
                var computedHash = argon2.GetBytes(32);

                // 4. Порівняння обчисленого хешу зі збереженим.
                // Використовуємо CryptographicOperations.FixedTimeEquals для захисту від timing attacks.
                return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
            }
            catch (FormatException ex) // Обробка помилки, якщо Base64 рядки некоректні.
            {
                Debug.WriteLine($"ValidatePassword: Error converting Base64 strings for user '{username}'. {ex.Message}");
                return false;
            }
            catch (Exception ex) // Інші можливі помилки Argon2.
            {
                Debug.WriteLine($"ValidatePassword: Error during hash computation for user '{username}'. {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}