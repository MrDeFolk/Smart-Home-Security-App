using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Albireo.Base32; // Використання Albireo.Base32 для кодування/декодування.
using OtpNet;         // Основна бібліотека для TOTP.

namespace Smart_Home_Project
{
    /// <summary>
    /// Сервіс для генерації та валідації одноразових паролів (TOTP)
    /// для двофакторної автентифікації.
    /// Використовує ConfigurationService для безпечного зберігання секретного ключа.
    /// </summary>
    public class MfaService
    {
        #region Константи та Поля (Constants & Fields)

        // Ключ, за яким секрет TOTP зберігається у конфігурації.
        private const string TotpSecretKeyName = "AdminTotpSecret";
        // Залежність від сервісу конфігурації.
        private readonly ConfigurationService configService;
        // Секретний ключ TOTP у вигляді байтового масиву. Nullable, бо ініціалізується в InitializeSecretKey.
        private byte[]? adminSecretKeyBytes;

        #endregion

        #region Конструктор

        public MfaService(ConfigurationService configurationService)
        {
            configService = configurationService;
            InitializeSecretKey(); // Ініціалізація або генерація ключа при старті.
        }

        #endregion

        #region Ініціалізація секретного ключа

        /// <summary>
        /// Намагається завантажити існуючий секретний ключ TOTP з конфігурації.
        /// Якщо ключ не знайдено або він пошкоджений, генерує новий та зберігає його.
        /// </summary>
        private void InitializeSecretKey()
        {
            // Завантаження секрету (зберігається у Base32 форматі).
            string? secretBase32 = configService.LoadDecryptedSetting(TotpSecretKeyName);

            if (string.IsNullOrEmpty(secretBase32))
            {
                // Генерація нового ключа, якщо старий не знайдено.
                Debug.WriteLine("TOTP Secret Key not found, generating a new one...");
                adminSecretKeyBytes = GenerateNewSecretKey();
                secretBase32 = Base32.Encode(adminSecretKeyBytes); // Кодування в Base32.
                                                                   // Збереження нового ключа у зашифрованому вигляді через DPAPI.
                configService.SaveEncryptedSetting(TotpSecretKeyName, secretBase32);
                Debug.WriteLine($"New TOTP Secret Key generated and saved (Base32): {secretBase32}");
            }
            else
            {
                // Декодування існуючого ключа з Base32.
                try
                {
                    adminSecretKeyBytes = Base32.Decode(secretBase32);
                    Debug.WriteLine("Existing TOTP Secret Key loaded successfully.");
                }
                catch (Exception ex) // Обробка помилок декодування.
                {
                    Debug.WriteLine($"Error decoding existing TOTP secret key: {ex.Message}. Generating a new one.");
                    // Якщо дані пошкоджені, генеруємо та зберігаємо новий ключ.
                    adminSecretKeyBytes = GenerateNewSecretKey();
                    secretBase32 = Base32.Encode(adminSecretKeyBytes);
                    configService.SaveEncryptedSetting(TotpSecretKeyName, secretBase32);
                }
            }

            // Фінальна перевірка на випадок критичної помилки.
            if (adminSecretKeyBytes == null || adminSecretKeyBytes.Length == 0)
            {
                Debug.WriteLine("CRITICAL ERROR: Failed to initialize TOTP secret key. Using fallback key.");
                adminSecretKeyBytes = GenerateNewSecretKey(); // Створення аварійного ключа.
            }
        }

        /// <summary>
        /// Генерує новий випадковий секретний ключ для TOTP.
        /// </summary>
        /// <param name="length">Довжина ключа в байтах (стандартно 20 байт / 160 біт).</param>
        private byte[] GenerateNewSecretKey(int length = 20)
        {
            byte[] secret = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(secret); // Заповнення випадковими даними.
            }
            return secret;
        }


        #endregion

        #region Генерація та Валідація TOTP

        /// <summary>
        /// Генерує поточний 6-значний TOTP код для користувача 'admin'.
        /// </summary>
        public string GenerateCode(string username)
        {
            // Перевірка ініціалізації ключа.
            if (adminSecretKeyBytes == null || adminSecretKeyBytes.Length == 0)
            {
                Debug.WriteLine("Error generating TOTP: Secret key is not initialized.");
                return "------"; // Повернення плейсхолдера помилки.
            }

            // У цій версії підтримується лише 'admin'.
            if (username.ToLower() != "admin")
            {
                return "N/A";
            }

            // Створення об'єкта Totp та обчислення коду.
            var totp = new Totp(adminSecretKeyBytes);
            return totp.ComputeTotp(); // Повертає поточний код.
        }

        /// <summary>
        /// Перевіряє, чи є введений код дійсним для користувача 'admin'.
        /// Враховує невелике вікно розсинхронізації часу.
        /// </summary>
        public bool ValidateCode(string username, string code)
        {
            if (adminSecretKeyBytes == null || adminSecretKeyBytes.Length == 0)
            {
                Debug.WriteLine("Error validating TOTP: Secret key is not initialized.");
                return false;
            }

            if (username.ToLower() != "admin" || string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var totp = new Totp(adminSecretKeyBytes);
            // Перевірка коду з урахуванням можливого відхилення часу
            // (перевіряє поточний, попередній та наступний інтервали).
            return totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
        }

        #endregion
    }
}