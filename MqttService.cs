using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security; // Для SslPolicyErrors
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics; // Для Debug

namespace Smart_Home_Project
{
    /// <summary>
    /// Сервіс для взаємодії з MQTT брокером.
    /// Керує підключенням (включаючи TLS), підпискою на топіки та публікацією повідомлень.
    /// </summary>
    public class MqttService : IDisposable
    {
        #region Поля, Події та Властивості

        private readonly IMqttClient mqttClient;
        // Кореневий сертифікат CA для валідації сервера. Nullable.
        private X509Certificate2? caCert;
        // Список топіків, на які потрібно підписатися після підключення.
        private readonly List<string> pendingSubscriptions = new List<string>();

        // Подія, що сповіщає про зміну стану підключення.
        public event Action<string>? ConnectionStateChanged;
        // Подія, що викликається при отриманні нового повідомлення.
        public event Func<MqttApplicationMessageReceivedEventArgs, Task>? MessageReceived;

        // Властивість, що показує поточний стан підключення.
        public bool IsConnected => mqttClient?.IsConnected ?? false;

        #endregion

        #region Конструктор

        public MqttService()
        {
            var factory = new MqttFactory(); // Фабрика для створення MQTT клієнта.
            mqttClient = factory.CreateMqttClient();

            // Налаштування обробників подій клієнта (версія 3.x API).
            mqttClient.UseConnectedHandler(async e => // Використовуємо async для очікування підписок
            {
                ConnectionStateChanged?.Invoke("Підключено до MQTT брокера (TLS)");
                // Автоматично підписуємось на збережені топіки.
                await SubscribePendingTopics();
            });

            mqttClient.UseDisconnectedHandler(e =>
            {
                ConnectionStateChanged?.Invoke("Відключено від MQTT брокера");
                // Можна додати логіку перепідключення тут, якщо потрібно.
            });

            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                // Передача отриманого повідомлення зовнішнім обробникам.
                OnMessageReceived(e);
            });
        }

        #endregion

        #region Керування підключенням

        /// <summary>
        /// Встановлює захищене TLS з'єднання з MQTT брокером,
        /// використовуючи автентифікацію за клієнтським сертифікатом.
        /// </summary>
        public async Task ConnectAsync()
        {
            if (IsConnected) return; // Не підключаємось, якщо вже підключені.

            // Thumbprint клієнтського сертифіката (має бути у сховищі поточного користувача).
            const string clientCertificateThumbprint = "41e3ddbc7cb9f4b1f9892e79434bafdb68bc51e7";
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var caCertPath = Path.Combine(baseDir, "certs", "ca.crt"); // Шлях до сертифіката CA.

            if (!File.Exists(caCertPath))
            {
                ConnectionStateChanged?.Invoke("Помилка: Файл кореневого сертифіката (ca.crt) не знайдено!");
                return;
            }

            try
            {
                caCert = new X509Certificate2(caCertPath); // Завантаження сертифіката CA.

                // Пошук клієнтського сертифіката у сховищі Windows.
                X509Certificate2? clientCert = null;
                using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    store.Open(OpenFlags.ReadOnly);
                    // Пошук за Thumbprint (без пробілів, у верхньому регістрі).
                    var certs = store.Certificates.Find(X509FindType.FindByThumbprint, clientCertificateThumbprint.Replace(" ", "").ToUpper(), false);
                    if (certs.Count > 0)
                    {
                        clientCert = certs[0]; // Беремо перший знайдений.
                    }
                    store.Close();
                }

                if (clientCert == null)
                {
                    ConnectionStateChanged?.Invoke($"Помилка: Сертифікат з Thumbprint '{clientCertificateThumbprint}' не знайдено у сховищі.");
                    return;
                }

                var clientCertificates = new List<X509Certificate> { clientCert };

                // Налаштування параметрів TLS (версія 3.x API).
                var tlsParameters = new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    SslProtocol = System.Security.Authentication.SslProtocols.Tls12, // Використовуємо TLS 1.2.
                    Certificates = clientCertificates, // Наш клієнтський сертифікат.
                    // Кастомний обробник валідації сертифіката сервера.
                    CertificateValidationCallback = (cert, chain, errors, options) =>
                    {
                        if (caCert == null) return false; // Немає CA для перевірки.
                        if (errors == SslPolicyErrors.None) return true; // Сертифікат валідний за системними правилами.

                        // Дозволяємо самопідписаний сертифікат, якщо він підписаний нашим CA.
                        if (errors == SslPolicyErrors.RemoteCertificateChainErrors)
                        {
                            // Налаштування політики перевірки ланцюжка.
                            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck; // Не перевіряємо відкликання.
                            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority; // Дозволяємо невідомий CA.
                            chain.ChainPolicy.ExtraStore.Add(caCert); // Додаємо наш CA до довірених для цієї перевірки.

                            var serverCert = new X509Certificate2(cert);
                            var isValid = chain.Build(serverCert); // Будуємо ланцюжок довіри.

                            if (isValid)
                            {
                                // Перевіряємо, чи кореневий сертифікат у ланцюжку - це наш CA.
                                var rootCert = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
                                return rootCert.Thumbprint == caCert.Thumbprint;
                            }
                        }
                        // В інших випадках помилок - відхиляємо сертифікат.
                        return false;
                    }
                };

                // Створення опцій підключення MQTT клієнта.
                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer("localhost", 8883) // Адреса та порт брокера.
                    .WithClientId($"smart-home-client-{Guid.NewGuid()}") // Унікальний ID клієнта.
                    .WithTls(tlsParameters) // Застосування налаштувань TLS.
                    .Build();

                ConnectionStateChanged?.Invoke("Підключення (TLS)...");
                // Спроба підключення.
                await mqttClient.ConnectAsync(options, CancellationToken.None);
            }
            catch (Exception ex) // Обробка помилок підключення.
            {
                ConnectionStateChanged?.Invoke($"Помилка TLS підключення: {ex.Message}");
                Debug.WriteLine($"TLS Connection Error: {ex}"); // Додатковий вивід в Debug.
            }
        }

        /// <summary>
        /// Розриває з'єднання з MQTT брокером.
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (!IsConnected) return;
            // ОНОВЛЕНО: Коректний виклик DisconnectAsync для v3.1.2
            await mqttClient.DisconnectAsync();
        }

        #endregion

        #region Обмін повідомленнями (Publish, Subscribe)

        /// <summary>
        /// Додає топік до списку очікування на підписку.
        /// Якщо клієнт вже підключений, підписується негайно.
        /// </summary>
        public Task AddSubscription(string topic)
        {
            if (!pendingSubscriptions.Contains(topic))
            {
                pendingSubscriptions.Add(topic);
            }
            // Негайна підписка, якщо вже є з'єднання.
            if (IsConnected)
            {
                return SubscribeAsync(topic);
            }
            return Task.CompletedTask; // Повертаємо завершену задачу, якщо підключення ще немає.
        }

        /// <summary>
        /// Виконує підписку на всі топіки зі списку очікування.
        /// Викликається автоматично після встановлення з'єднання.
        /// </summary>
        private async Task SubscribePendingTopics()
        {
            if (!IsConnected || !pendingSubscriptions.Any()) return;

            // Створюємо копію списку та очищуємо оригінал, щоб уникнути повторних підписок.
            var topicsToSubscribe = pendingSubscriptions.Distinct().ToList();
            pendingSubscriptions.Clear();

            Debug.WriteLine($"Subscribing to {topicsToSubscribe.Count} topics...");
            foreach (var topic in topicsToSubscribe)
            {
                await SubscribeAsync(topic); // Виклик фактичної підписки.
            }
        }


        /// <summary>
        /// Виконує підписку на вказаний MQTT топік.
        /// </summary>
        private async Task SubscribeAsync(string topic)
        {
            if (!IsConnected) return;
            try
            {
                // Використання MqttTopicFilterBuilder для створення фільтра підписки.
                await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
                Debug.WriteLine($"Subscribed to topic: {topic}");
            }
            catch (Exception ex) // Обробка помилок підписки.
            {
                Debug.WriteLine($"Failed to subscribe to topic {topic}: {ex.Message}");
            }
        }

        /// <summary>
        /// Публікує повідомлення у вказаний MQTT топік.
        /// </summary>
        public async Task PublishAsync(string topic, string payload)
        {
            if (!IsConnected)
            {
                ConnectionStateChanged?.Invoke("Неможливо надіслати: клієнт не підключено.");
                return;
            }

            // Створення MQTT повідомлення за допомогою MqttApplicationMessageBuilder.
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(payload)) // Кодуємо рядок у UTF8 байти.
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce) // Рівень QoS 2.
                .Build();

            // Публікація повідомлення.
            await mqttClient.PublishAsync(message, CancellationToken.None);
        }

        #endregion

        #region Внутрішні обробники подій

        /// <summary>
        /// Внутрішній обробник, що викликає зовнішню подію MessageReceived асинхронно.
        /// </summary>
        private void OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            // Запускаємо зовнішній обробник в окремому потоці, щоб не блокувати MQTT клієнт.
            _ = Task.Run(() => MessageReceived?.Invoke(e));
        }

        #endregion

        #region Звільнення ресурсів (IDisposable)

        /// <summary>
        /// Звільняє ресурси MQTT клієнта.
        /// </summary>
        public void Dispose()
        {
            mqttClient?.Dispose();
            GC.SuppressFinalize(this); // Повідомляємо збирачу сміття, що фіналізація не потрібна.
        }

        #endregion
    }
}