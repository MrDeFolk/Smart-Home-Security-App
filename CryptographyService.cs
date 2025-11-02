using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Smart_Home_Project
{
    /// <summary>
    /// Сервіс, що реалізує криптографічні операції (AES, RSA, гібридне шифрування).
    /// </summary>
    public class CryptographyService
    {
        #region Поля (Fields)

        private readonly RSA rsaPublicKeyProvider;
        private readonly RSA rsaPrivateKeyProvider;
        private readonly Stopwatch stopwatch;
        // Списки для зберігання історії часу виконання операцій.
        private readonly List<double> encryptionTimes = new List<double>();
        private readonly List<double> decryptionTimes = new List<double>();
        private const int MaxHistorySize = 10; // Максимальна кількість записів в історії.

        #endregion

        #region Властивості (Properties)

        // Час виконання останньої операції шифрування.
        public double LastEncryptionTimeMs { get; private set; }
        // Час виконання останньої операції дешифрування.
        public double LastDecryptionTimeMs { get; private set; }
        // Середній час шифрування за збережену історію.
        public double AverageEncryptionTimeMs => encryptionTimes.Any() ? encryptionTimes.Average() : 0.0;
        // Середній час дешифрування за збережену історію.
        public double AverageDecryptionTimeMs => decryptionTimes.Any() ? decryptionTimes.Average() : 0.0;
        // Копія історії часу шифрування.
        public List<double> EncryptionTimeHistory => new List<double>(encryptionTimes);
        // Копія історії часу дешифрування.
        public List<double> DecryptionTimeHistory => new List<double>(decryptionTimes);


        #endregion


        #region Конструктор

        public CryptographyService()
        {
            stopwatch = new Stopwatch(); // Ініціалізація таймера.

            // Формування шляхів до файлів ключів RSA.
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var publicKeyPath = Path.Combine(baseDir, "certs", "device_public.pem");
            var privateKeyPath = Path.Combine(baseDir, "certs", "device_private.pem");

            // Перевірка наявності файлів.
            if (!File.Exists(publicKeyPath) || !File.Exists(privateKeyPath))
            {
                throw new FileNotFoundException("Файли RSA-ключів (.pem) не знайдено! Перевірте папку 'certs'.");
            }

            // Завантаження ключів з PEM-файлів.
            string publicKeyPem = File.ReadAllText(publicKeyPath);
            string privateKeyPem = File.ReadAllText(privateKeyPath);

            rsaPublicKeyProvider = RSA.Create();
            rsaPublicKeyProvider.ImportFromPem(publicKeyPem);

            rsaPrivateKeyProvider = RSA.Create();
            rsaPrivateKeyProvider.ImportFromPem(privateKeyPem);
        }

        #endregion

        #region Гібридне шифрування та дешифрування (AES + RSA)

        /// <summary>
        /// Шифрує дані за гібридною схемою: генерує сесійний AES-ключ, шифрує ним дані,
        /// а сам ключ шифрує публічним RSA-ключем.
        /// Вимірює час виконання операції.
        /// </summary>
        /// <param name="plainText">Вхідний текст для шифрування.</param>
        /// <returns>Кортеж з компонентами для передачі (зашифрований ключ, IV, зашифровані дані) або null у разі помилки.</returns>
        public (string encryptedSessionKey, string iv, string encryptedData)? EncryptHybrid(string plainText)
        {
            stopwatch.Restart(); // Запуск/перезапуск таймера.
            try
            {
                // 1. Генерація сесійного AES-ключа та IV.
                using var aes = Aes.Create();
                aes.KeySize = 256; // Використовуємо AES-256.
                aes.GenerateKey();
                aes.GenerateIV();
                byte[] sessionKey = aes.Key;
                byte[] iv = aes.IV;

                // 2. Шифрування даних сесійним ключем.
                byte[] encryptedDataBytes = EncryptAes(Encoding.UTF8.GetBytes(plainText), sessionKey, iv);

                // 3. Шифрування сесійного ключа публічним RSA-ключем.
                byte[] encryptedSessionKeyBytes = rsaPublicKeyProvider.Encrypt(sessionKey, RSAEncryptionPadding.OaepSHA256);

                stopwatch.Stop(); // Зупинка таймера.
                LastEncryptionTimeMs = stopwatch.Elapsed.TotalMilliseconds; // Збереження результату.
                AddAndTrimHistory(encryptionTimes, LastEncryptionTimeMs); // Додавання до історії.
                Debug.WriteLine($"--- Hybrid Encryption completed in {LastEncryptionTimeMs:F4} ms ---");

                // 4. Конвертація результатів у Base64 для передачі.
                return (
                    Convert.ToBase64String(encryptedSessionKeyBytes),
                    Convert.ToBase64String(iv),
                    Convert.ToBase64String(encryptedDataBytes)
                );
            }
            catch (Exception ex) // Обробка можливих помилок криптографії.
            {
                Debug.WriteLine($"Hybrid Encryption Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Розшифровує дані за гібридною схемою: розшифровує сесійний AES-ключ приватним RSA-ключем,
        /// а потім розшифровує дані за допомогою цього ключа та IV.
        /// Вимірює час виконання операції.
        /// </summary>
        /// <param name="encryptedSessionKey">Зашифрований сесійний ключ (Base64).</param>
        /// <param name="iv">Вектор ініціалізації (Base64).</param>
        /// <param name="encryptedData">Зашифровані дані (Base64).</param>
        /// <returns>Розшифрований текст або null у разі помилки.</returns>
        public string? DecryptHybrid(string encryptedSessionKey, string iv, string encryptedData)
        {
            stopwatch.Restart();
            try
            {
                // 1. Конвертація з Base64 у байти.
                byte[] encryptedSessionKeyBytes = Convert.FromBase64String(encryptedSessionKey);
                byte[] ivBytes = Convert.FromBase64String(iv);
                byte[] encryptedDataBytes = Convert.FromBase64String(encryptedData);

                // 2. Розшифрування сесійного ключа приватним RSA-ключем.
                byte[] sessionKeyBytes = rsaPrivateKeyProvider.Decrypt(encryptedSessionKeyBytes, RSAEncryptionPadding.OaepSHA256);

                // 3. Розшифрування даних сесійним ключем.
                byte[] decryptedDataBytes = DecryptAes(encryptedDataBytes, sessionKeyBytes, ivBytes);

                stopwatch.Stop();
                LastDecryptionTimeMs = stopwatch.Elapsed.TotalMilliseconds; // Збереження результату.
                AddAndTrimHistory(decryptionTimes, LastDecryptionTimeMs); // Додавання до історії.
                Debug.WriteLine($"--- Hybrid Decryption completed in {LastDecryptionTimeMs:F4} ms ---");

                // 4. Конвертація байтів у рядок UTF8.
                return Encoding.UTF8.GetString(decryptedDataBytes);
            }
            catch (Exception ex) // Обробка помилок (неправильний ключ, пошкоджені дані тощо).
            {
                Debug.WriteLine($"Hybrid Decryption Error: {ex.Message}");
                return null;
            }
        }


        #endregion

        #region Базові методи AES

        /// <summary>
        /// Виконує шифрування AES-256 CBC PKCS7.
        /// </summary>
        private byte[] EncryptAes(byte[] dataToEncrypt, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC; // Режим шифрування (Cipher Block Chaining).
            aes.Padding = PaddingMode.PKCS7; // Стандартний режим доповнення блоків.

            using var memoryStream = new MemoryStream();
            // Створення потоку для шифрування.
            using var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(dataToEncrypt, 0, dataToEncrypt.Length);
            cryptoStream.FlushFinalBlock(); // Завершення шифрування останнього блоку.
            return memoryStream.ToArray(); // Повернення зашифрованих байтів.
        }

        /// <summary>
        /// Виконує дешифрування AES-256 CBC PKCS7.
        /// </summary>
        private byte[] DecryptAes(byte[] dataToDecrypt, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var memoryStream = new MemoryStream();
            // Створення потоку для дешифрування.
            using var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(dataToDecrypt, 0, dataToDecrypt.Length);
            cryptoStream.FlushFinalBlock(); // Завершення дешифрування.
            return memoryStream.ToArray(); // Повернення розшифрованих байтів.
        }

        #endregion

        #region Допоміжні методи

        /// <summary>
        /// Додає нове значення до списку історії та видаляє найстаріше, якщо перевищено ліміт.
        /// </summary>
        private void AddAndTrimHistory(List<double> historyList, double newValue)
        {
            historyList.Add(newValue);
            if (historyList.Count > MaxHistorySize)
            {
                historyList.RemoveAt(0); // Видалення першого (найстарішого) елемента.
            }
        }

        #endregion
    }
}