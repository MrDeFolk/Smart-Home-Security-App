using System;
using System.Collections.Generic; // Для Dictionary
using System.Diagnostics;
using System.IO;
using System.Linq; // Для Select
using System.Security.Cryptography; // Для ProtectedData та CryptographicException
using System.Text;

namespace Smart_Home_Project
{
    /// <summary>
    /// Сервіс для безпечного зберігання та завантаження конфігураційних даних
    /// з використанням Windows Data Protection API (DPAPI).
    /// Дані шифруються прив'язано до поточного користувача Windows.
    /// </summary>
    public class ConfigurationService
    {
        #region Поля (Fields)

        // Шлях до файлу, де зберігаються зашифровані налаштування.
        private readonly string configFilePath;
        // Додаткова ентропія (сіль) для DPAPI, ускладнює атаки.
        // Має бути унікальною для програми, але не обов'язково секретною.
        private readonly byte[] entropy = Encoding.UTF8.GetBytes("SmartHomeProjectEntropyValue123!");

        #endregion

        #region Конструктор

        public ConfigurationService()
        {
            // Файл конфігурації зберігається поруч з .exe файлом програми.
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            configFilePath = Path.Combine(baseDir, "config.dat");
        }

        #endregion

        #region Публічні методи

        /// <summary>
        /// Зберігає рядок налаштування у зашифрованому вигляді у файл конфігурації.
        /// Якщо налаштування з таким ключем вже існує, воно буде перезаписано.
        /// </summary>
        /// <param name="key">Унікальний ключ налаштування.</param>
        /// <param name="value">Значення налаштування для збереження.</param>
        public void SaveEncryptedSetting(string key, string value)
        {
            try
            {
                // Завантажуємо поточні налаштування.
                var settings = LoadAllEncryptedSettings();
                // Шифруємо нове значення та додаємо/оновлюємо його у словнику.
                settings[key] = EncryptString(value);
                // Зберігаємо оновлений словник назад у файл.
                SaveAllEncryptedSettings(settings);
            }
            catch (Exception ex) // Обробка можливих помилок шифрування або запису файлу.
            {
                Debug.WriteLine($"Error saving encrypted setting '{key}': {ex.Message}");
                // У продакшн-коді тут може бути логування помилки у файл або систему моніторингу.
            }
        }

        /// <summary>
        /// Завантажує та розшифровує рядок налаштування за його ключем.
        /// </summary>
        /// <param name="key">Ключ налаштування.</param>
        /// <returns>Розшифроване значення або null, якщо ключ не знайдено,
        /// файл пошкоджено, або виникла помилка дешифрування.</returns>
        public string? LoadDecryptedSetting(string key)
        {
            try
            {
                // Завантажуємо всі зашифровані налаштування.
                var settings = LoadAllEncryptedSettings();
                // Шукаємо потрібний ключ.
                if (settings.TryGetValue(key, out string? encryptedValue))
                {
                    // Якщо знайдено, розшифровуємо значення.
                    return DecryptString(encryptedValue);
                }
                return null; // Ключ не знайдено.
            }
            catch (Exception ex) // Обробка можливих помилок читання файлу або дешифрування.
            {
                Debug.WriteLine($"Error loading decrypted setting '{key}': {ex.Message}");
                return null; // Повертаємо null у разі будь-якої помилки.
            }
        }

        #endregion

        #region Приватні допоміжні методи

        /// <summary>
        /// Завантажує всі зашифровані пари ключ-значення з файлу конфігурації.
        /// </summary>
        /// <returns>Словник з зашифрованими налаштуваннями.</returns>
        private Dictionary<string, string> LoadAllEncryptedSettings()
        {
            var settings = new Dictionary<string, string>();
            if (!File.Exists(configFilePath))
            {
                return settings; // Повертаємо порожній словник, якщо файл не існує.
            }

            try
            {
                // Читаємо всі рядки з файлу.
                var lines = File.ReadAllLines(configFilePath);
                foreach (var line in lines)
                {
                    // Розділяємо рядок на ключ та значення по першому символу '='.
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]))
                    {
                        settings[parts[0].Trim()] = parts[1]; // Додаємо у словник.
                    }
                }
            }
            catch (Exception ex) // Обробка помилок читання файлу.
            {
                Debug.WriteLine($"Error reading config file '{configFilePath}': {ex.Message}");
                // Повертаємо поточний (можливо, частково заповнений) словник.
            }
            return settings;
        }

        /// <summary>
        /// Зберігає весь словник зашифрованих налаштувань у файл конфігурації.
        /// Кожна пара ключ-значення записується в окремий рядок формату "ключ=значення".
        /// </summary>
        private void SaveAllEncryptedSettings(Dictionary<string, string> settings)
        {
            try
            {
                // Формуємо рядки для запису у файл.
                var lines = settings.Select(kvp => $"{kvp.Key}={kvp.Value}");
                // Перезаписуємо файл повністю.
                File.WriteAllLines(configFilePath, lines);
            }
            catch (Exception ex) // Обробка помилок запису файлу.
            {
                Debug.WriteLine($"Error writing config file '{configFilePath}': {ex.Message}");
            }
        }

        /// <summary>
        /// Шифрує вхідний рядок за допомогою DPAPI (ProtectedData).
        /// Використовує DataProtectionScope.CurrentUser та додаткову ентропію.
        /// </summary>
        /// <returns>Зашифрований рядок у форматі Base64.</returns>
        private string EncryptString(string plainText)
        {
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            // Шифрування даних для поточного користувача з використанням ентропії.
            byte[] encryptedData = ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData); // Конвертація у Base64 для збереження у текстовий файл.
        }

        /// <summary>
        /// Розшифровує рядок (у форматі Base64), зашифрований за допомогою DPAPI.
        /// </summary>
        /// <returns>Розшифрований рядок або null у разі помилки.</returns>
        private string? DecryptString(string encryptedText)
        {
            try
            {
                byte[] encryptedData = Convert.FromBase64String(encryptedText); // З Base64 у байти.
                                                                                // Дешифрування даних для поточного користувача з тією ж ентропією.
                byte[] decryptedData = ProtectedData.Unprotect(encryptedData, entropy, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedData); // З байтів у рядок UTF8.
            }
            catch (CryptographicException ex) // Помилка дешифрування DPAPI.
            {
                // Найчастіші причини: спроба дешифрувати на іншому комп'ютері,
                // під іншим користувачем, або пошкодження даних.
                Debug.WriteLine($"DPAPI Decryption Error: {ex.Message}. Possible reasons: Trying to decrypt on a different machine, under a different user, or the data is corrupted.");
                return null;
            }
            catch (FormatException ex) // Помилка декодування Base64.
            {
                Debug.WriteLine($"Base64 Decoding Error during decryption: {ex.Message}");
                return null;
            }
            catch (Exception ex) // Інші можливі помилки.
            {
                Debug.WriteLine($"Generic Decryption Error: {ex.Message}");
                return null;
            }
        }

        #endregion
    }
}