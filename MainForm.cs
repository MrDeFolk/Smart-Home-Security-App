using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using MQTTnet;
using Timer = System.Windows.Forms.Timer;

namespace Smart_Home_Project
{
    /// <summary>
    /// Головна форма додатку, що керує відображенням та взаємодією користувача.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Сервіси (Dependencies)

        private readonly DatabaseService dbService;
        private readonly SecurityService securityService;
        private readonly ConfigurationService configService;
        private readonly MfaService mfaService;
        private readonly MqttService mqttService;
        private readonly CryptographyService cryptoService;

        #endregion

        #region UI Компоненти (Controls)

        // Таймер для візуального ефекту підсвічування кнопки.
        private Timer statusHighlightTimer = null!;

        // Змінні для контролів, створених у дизайнері, оголошуються в MainForm.Designer.cs

        #endregion

        #region Стан форми (State)

        // Поточний вибраний пристрій у TreeView.
        private Device selectedDevice = null!;
        // Ім'я користувача, який увійшов у систему.
        private string loggedInUsername = null!;

        #endregion

        #region Конструктор

        public MainForm()
        {
            // Ініціалізація сервісів для роботи програми.
            dbService = new DatabaseService();
            securityService = new SecurityService(dbService);
            configService = new ConfigurationService();
            mfaService = new MfaService(configService);
            cryptoService = new CryptographyService();
            mqttService = new MqttService();

            // Ініціалізація UI компонентів (з MainForm.Designer.cs).
            InitializeComponent();
            // Додаткові налаштування UI та прив'язка подій.
            InitializeCustomComponents();

            // Перевірка та створення користувача за замовчуванням (якщо його немає).
            CheckAndCreateDefaultUser();

            // Підписка на події форми та сервісів.
            this.FormClosing += MainForm_FormClosing;
            mqttService.ConnectionStateChanged += MqttService_ConnectionStateChanged;
            mqttService.MessageReceived += MqttService_MessageReceived;

            // Ініціалізація таймера підсвічування.
            InitializeHighlightTimer();
        }


        #endregion

        #region Ініціалізація UI

        /// <summary>
        /// Виконує додаткові налаштування UI компонентів (меню, статус-бар)
        /// та прив'язує обробники подій до елементів, створених у дизайнері.
        /// </summary>
        private void InitializeCustomComponents()
        {
            this.Text = "Smart Home Security Control Panel";
            this.MinimumSize = new Size(640, 480); // Мінімально допустимий розмір вікна

            // Налаштування MenuStrip (головне меню).
            if (menuStrip != null)
            {
                var fileMenu = new ToolStripMenuItem("Файл");
                fileMenu.DropDownItems.Add("Вийти з системи", null, (s, e) => Logout());
                fileMenu.DropDownItems.Add("Вихід", null, (s, e) => this.Close());

                var viewMenu = new ToolStripMenuItem("Перегляд");
                viewMenu.DropDownItems.Add("Панель керування", null, (s, e) => ShowDashboardView());
                viewMenu.DropDownItems.Add("Логи безпеки", null, (s, e) => ShowLogsView());
                viewMenu.DropDownItems.Add("Статистика продуктивності", null, (s, e) => ShowPerformanceView());

                var helpMenu = new ToolStripMenuItem("Довідка");
                helpMenu.DropDownItems.Add("Про програму", null, (s, e) => ShowAboutProgramView());
                helpMenu.DropDownItems.Add("Про автора", null, (s, e) => ShowAboutAuthorView());

                // Додавання пунктів меню (якщо їх немає в дизайнері).
                if (!menuStrip.Items.ContainsKey("fileMenu")) { fileMenu.Name = "fileMenu"; menuStrip.Items.Add(fileMenu); }
                if (!menuStrip.Items.ContainsKey("viewMenu")) { viewMenu.Name = "viewMenu"; menuStrip.Items.Add(viewMenu); }
                if (!menuStrip.Items.ContainsKey("helpMenu")) { helpMenu.Name = "helpMenu"; menuStrip.Items.Add(helpMenu); }

                menuStrip.Visible = false; // Початково приховано до входу користувача.
            }


            // Налаштування StatusStrip (панель стану).
            if (statusBar != null && statusLabel != null && encryptionTimeLabel != null && decryptionTimeLabel != null)
            {
                statusLabel.Text = "Готово";
                encryptionTimeLabel.Text = "Ост. шифр.: - мс"; encryptionTimeLabel.Spring = true; encryptionTimeLabel.TextAlign = ContentAlignment.MiddleRight;
                decryptionTimeLabel.Text = "Ост. дешифр.: - мс"; decryptionTimeLabel.Spring = true; decryptionTimeLabel.TextAlign = ContentAlignment.MiddleRight;
            }

            // Прив'язка обробників подій до контролів, створених у дизайнері.
            // Панель логіну:
            if (loginButton != null) loginButton.Click += LoginButton_Click;
            if (togglePasswordButton != null) togglePasswordButton.Click += TogglePasswordButton_Click;
            if (usernameTextBox != null) usernameTextBox.KeyPress += InputValidation_KeyPress;
            if (passwordTextBox != null) passwordTextBox.KeyPress += InputValidation_KeyPress;
            // Панель TOTP:
            if (confirmTotpButton != null) confirmTotpButton.Click += ConfirmTotpButton_Click;
            if (resendTotpButton != null) resendTotpButton.Click += ResendTotpButton_Click;
            // Панель керування:
            if (roomsTreeView != null) roomsTreeView.AfterSelect += RoomsTreeView_AfterSelect;
            if (toggleDeviceButton != null) toggleDeviceButton.Click += ToggleDeviceButton_Click;
            if (setTemperatureButton != null) setTemperatureButton.Click += SetTemperatureButton_Click;
            if (simulateResponseButton != null) simulateResponseButton.Click += SimulateResponseButton_Click;
            // Інші панелі не мають прямих обробників подій.


            // Відображення початкового екрану (логін).
            ShowLoginView();
        }

        /// <summary>
        /// Ініціалізує таймер для короткочасного підсвічування кнопки.
        /// </summary>
        private void InitializeHighlightTimer()
        {
            statusHighlightTimer = new Timer();
            statusHighlightTimer.Interval = 500; // Тривалість ефекту.
            statusHighlightTimer.Tick += (sender, e) =>
            {
                statusHighlightTimer.Stop();
                // Повернення стандартного кольору кнопки.
                if (simulateResponseButton != null)
                {
                    simulateResponseButton.BackColor = SystemColors.Control;
                }
            };
        }


        #endregion

        #region Керування видимістю панелей (View Management)

        /// <summary>
        /// Відображає панель логіну та приховує інші.
        /// </summary>
        private void ShowLoginView()
        {
            if (menuStrip != null) menuStrip.Visible = false;
            if (statusBar != null) statusBar.Visible = false; // Приховуємо статус-бар
            if (loginPanel != null) loginPanel.Visible = true;
            if (totpPanel != null) totpPanel.Visible = false;
            if (dashboardPanel != null) dashboardPanel.Visible = false;
            if (logsPanel != null) logsPanel.Visible = false;
            if (performancePanel != null) performancePanel.Visible = false;
            if (aboutProgramPanel != null) aboutProgramPanel.Visible = false;
            if (aboutAuthorPanel != null) aboutAuthorPanel.Visible = false;

            // Очищення полів та встановлення фокусу.
            if (usernameTextBox != null) usernameTextBox.Text = "";
            if (passwordTextBox != null) passwordTextBox.Text = "";
            if (usernameTextBox != null) usernameTextBox.Focus();
        }

        /// <summary>
        /// Відображає панель введення TOTP коду.
        /// </summary>
        private void ShowTotpView()
        {
            if (menuStrip != null) menuStrip.Visible = false;
            if (statusBar != null) statusBar.Visible = false;
            if (loginPanel != null) loginPanel.Visible = false;
            if (totpPanel != null) totpPanel.Visible = true;
            if (dashboardPanel != null) dashboardPanel.Visible = false;
            if (logsPanel != null) logsPanel.Visible = false;
            if (performancePanel != null) performancePanel.Visible = false;
            if (aboutProgramPanel != null) aboutProgramPanel.Visible = false;
            if (aboutAuthorPanel != null) aboutAuthorPanel.Visible = false;

            if (totpTextBox != null) totpTextBox.Text = "";
            if (totpTextBox != null) totpTextBox.Focus();
            GenerateAndDisplayNewTotpCode(); // Генеруємо та показуємо (для тесту) перший код.
        }

        /// <summary>
        /// Відображає головну панель керування пристроями.
        /// </summary>
        private async void ShowDashboardView()
        {
            if (menuStrip != null) menuStrip.Visible = true;
            if (statusBar != null) statusBar.Visible = true; // Показуємо статус-бар
            if (loginPanel != null) loginPanel.Visible = false;
            if (totpPanel != null) totpPanel.Visible = false;
            if (dashboardPanel != null) dashboardPanel.Visible = true;
            if (logsPanel != null) logsPanel.Visible = false;
            if (performancePanel != null) performancePanel.Visible = false;
            if (aboutProgramPanel != null) aboutProgramPanel.Visible = false;
            if (aboutAuthorPanel != null) aboutAuthorPanel.Visible = false;

            LoadRoomsAndDevices(); // Завантажуємо структуру кімнат/пристроїв.
                                   // Підключаємось до MQTT та підписуємось на топіки.
            if (!mqttService.IsConnected)
            {
                await mqttService.ConnectAsync();
                if (mqttService.IsConnected)
                {
                    await mqttService.AddSubscription("smarthome/devices/+/status_hybrid");
                }
            }
            else
            {
                // Якщо вже підключені, просто перевіряємо підписку.
                await mqttService.AddSubscription("smarthome/devices/+/status_hybrid");
            }
        }

        /// <summary>
        /// Відображає панель з логами безпеки.
        /// </summary>
        private void ShowLogsView()
        {
            if (menuStrip != null) menuStrip.Visible = true;
            if (statusBar != null) statusBar.Visible = true;
            if (loginPanel != null) loginPanel.Visible = false;
            if (totpPanel != null) totpPanel.Visible = false;
            if (dashboardPanel != null) dashboardPanel.Visible = false;
            if (logsPanel != null) logsPanel.Visible = true;
            if (performancePanel != null) performancePanel.Visible = false;
            if (aboutProgramPanel != null) aboutProgramPanel.Visible = false;
            if (aboutAuthorPanel != null) aboutAuthorPanel.Visible = false;
            LoadSecurityLogs(); // Завантажуємо логи з БД.
        }

        /// <summary>
        /// Відображає панель статистики продуктивності криптографії.
        /// </summary>
        private void ShowPerformanceView()
        {
            if (menuStrip != null) menuStrip.Visible = true;
            if (statusBar != null) statusBar.Visible = true;
            if (loginPanel != null) loginPanel.Visible = false;
            if (totpPanel != null) totpPanel.Visible = false;
            if (dashboardPanel != null) dashboardPanel.Visible = false;
            if (logsPanel != null) logsPanel.Visible = false;
            if (performancePanel != null) performancePanel.Visible = true;
            if (aboutProgramPanel != null) aboutProgramPanel.Visible = false;
            if (aboutAuthorPanel != null) aboutAuthorPanel.Visible = false;
            UpdatePerformanceLabels(); // Оновлюємо дані статистики.
        }

        /// <summary>
        /// Відображає панель "Про програму".
        /// </summary>
        private void ShowAboutProgramView()
        {
            if (menuStrip != null) menuStrip.Visible = true;
            if (statusBar != null) statusBar.Visible = true;
            if (loginPanel != null) loginPanel.Visible = false;
            if (totpPanel != null) totpPanel.Visible = false;
            if (dashboardPanel != null) dashboardPanel.Visible = false;
            if (logsPanel != null) logsPanel.Visible = false;
            if (performancePanel != null) performancePanel.Visible = false;
            if (aboutProgramPanel != null) aboutProgramPanel.Visible = true;
            if (aboutAuthorPanel != null) aboutAuthorPanel.Visible = false;
        }

        /// <summary>
        /// Відображає панель "Про автора".
        /// </summary>
        private void ShowAboutAuthorView()
        {
            if (menuStrip != null) menuStrip.Visible = true;
            if (statusBar != null) statusBar.Visible = true;
            if (loginPanel != null) loginPanel.Visible = false;
            if (totpPanel != null) totpPanel.Visible = false;
            if (dashboardPanel != null) dashboardPanel.Visible = false;
            if (logsPanel != null) logsPanel.Visible = false;
            if (performancePanel != null) performancePanel.Visible = false;
            if (aboutProgramPanel != null) aboutProgramPanel.Visible = false;
            if (aboutAuthorPanel != null) aboutAuthorPanel.Visible = true;
        }


        #endregion

        #region Обробники подій UI та дій користувача

        /// <summary>
        /// Обробляє натискання кнопки "Далі" на екрані логіну.
        /// Перевіряє логін/пароль та переходить до екрану TOTP або показує помилку.
        /// </summary>
        private void LoginButton_Click(object? sender, EventArgs e)
        {
            if (usernameTextBox == null || passwordTextBox == null) return;

            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Будь ласка, введіть логін та пароль.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Валідація пароля через SecurityService.
            bool isAuthenticated = securityService.ValidatePassword(username, password);

            if (isAuthenticated)
            {
                loggedInUsername = username; // Зберігаємо ім'я користувача.
                dbService.LogSecurityEvent("Login Success", "User successfully entered password.", username);
                ShowTotpView(); // Перехід до 2FA.
            }
            else
            {
                dbService.LogSecurityEvent("Login Failed", "Invalid username or password.", username);
                MessageBox.Show("Невірний логін або пароль.", "Помилка автентифікації", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обробляє натискання кнопки "Увійти" на екрані TOTP.
        /// Перевіряє код та переходить до панелі керування або показує помилку.
        /// </summary>
        private void ConfirmTotpButton_Click(object? sender, EventArgs e)
        {
            if (totpTextBox == null) return;
            string totpCode = totpTextBox.Text;
            if (loggedInUsername == null) return; // Має бути встановлено на попередньому кроці.

            // Валідація TOTP коду.
            bool isTotpValid = mfaService.ValidateCode(loggedInUsername, totpCode);

            if (isTotpValid)
            {
                dbService.LogSecurityEvent("2FA Success", "User successfully entered TOTP.", loggedInUsername);
                ShowDashboardView(); // Успішний вхід.
            }
            else
            {
                dbService.LogSecurityEvent("2FA Failed", "Invalid TOTP entered.", loggedInUsername);
                MessageBox.Show("Невірний код.", "Помилка 2FA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обробляє натискання кнопки "Надіслати знову" (генерує новий тестовий код).
        /// </summary>
        private void ResendTotpButton_Click(object? sender, EventArgs e)
        {
            GenerateAndDisplayNewTotpCode();
        }

        /// <summary>
        /// Обробляє вибір вузла у дереві кімнат/пристроїв.
        /// Оновлює панель деталей та показує відповідні елементи керування.
        /// </summary>
        private void RoomsTreeView_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            // Перевірки на null для безпеки.
            if (toggleDeviceButton == null || temperatureSelector == null || tempUnitLabel == null || setTemperatureButton == null || simulateResponseButton == null || deviceDetailsGroupBox == null) return;

            // Сховуємо специфічні контролі за замовчуванням.
            toggleDeviceButton.Visible = false;
            temperatureSelector.Visible = false;
            tempUnitLabel.Visible = false;
            setTemperatureButton.Visible = false;
            simulateResponseButton.Enabled = false;


            if (e.Node?.Tag is Device device) // Якщо вибраний вузол - це пристрій.
            {
                selectedDevice = device;
                UpdateDeviceDetails(); // Оновлюємо відображення деталей.
                simulateResponseButton.Enabled = true; // Вмикаємо кнопку симуляції.

                // Показуємо відповідні контролі залежно від типу пристрою.
                if (device.DeviceType == "Лампа")
                {
                    toggleDeviceButton.Visible = true;
                }
                else if (device.DeviceType == "Термостат")
                {
                    temperatureSelector.Visible = true;
                    tempUnitLabel.Visible = true;
                    setTemperatureButton.Visible = true;
                }
            }
            else // Якщо вибрано кімнату або нічого.
            {
                selectedDevice = null!; // Скидаємо вибраний пристрій.
                deviceDetailsGroupBox.Visible = false; // Сховуємо панель деталей.
            }
        }

        /// <summary>
        /// Обробляє натискання кнопки "Переключити" для лампи.
        /// Шифрує та надсилає команду зміни статусу через MQTT.
        /// </summary>
        private async void ToggleDeviceButton_Click(object? sender, EventArgs e)
        {
            if (selectedDevice == null || loggedInUsername == null || selectedDevice.DeviceType != "Лампа") return;

            string newStatus = selectedDevice.Status == "Ввімкнено" ? "Вимкнено" : "Ввімкнено";

            // Формування JSON команди.
            var commandPayload = new { deviceId = selectedDevice.Id, status = newStatus };
            string jsonPayload = JsonSerializer.Serialize(commandPayload);

            // Гібридне шифрування команди.
            var encryptedPackage = cryptoService.EncryptHybrid(jsonPayload);
            if (encryptedPackage == null)
            {
                Debug.WriteLine("Помилка гібридного шифрування!");
                MessageBox.Show("Помилка шифрування команди.", "Критична помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Оновлення статистики.
            UpdateEncryptionTimeLabel(cryptoService.LastEncryptionTimeMs);
            UpdatePerformanceLabels();


            // Формування фінального повідомлення для MQTT.
            var finalPayloadContainer = new
            {
                key = encryptedPackage.Value.encryptedSessionKey, // Зашифрований сесійний ключ
                iv = encryptedPackage.Value.iv,                   // Вектор ініціалізації
                payload = encryptedPackage.Value.encryptedData    // Зашифровані дані команди
            };
            string finalPayload = JsonSerializer.Serialize(finalPayloadContainer);
            string topic = $"smarthome/devices/{selectedDevice.Id}/command_hybrid";

            // Публікація повідомлення.
            await mqttService.PublishAsync(topic, finalPayload);

            // Оновлення статусу локально для миттєвого відображення.
            dbService.UpdateDeviceStatus(selectedDevice.Id, newStatus);
            selectedDevice.Status = newStatus;
            UpdateDeviceDetails();

            Debug.WriteLine($"MQTT PUBLISH (HYBRID - Lamp): Topic='{topic}'");
            Debug.WriteLine($"---> Original Payload: {jsonPayload}");
            dbService.LogSecurityEvent("Device Toggled (Hybrid)", $"Device '{selectedDevice.Name}' status changed to '{newStatus}'. Hybrid encrypted command sent.", loggedInUsername);
        }

        /// <summary>
        /// Обробляє натискання кнопки "Встановити" для термостата.
        /// Шифрує та надсилає команду зміни температури через MQTT.
        /// </summary>
        private async void SetTemperatureButton_Click(object? sender, EventArgs e)
        {
            if (selectedDevice == null || loggedInUsername == null || selectedDevice.DeviceType != "Термостат" || temperatureSelector == null) return;

            decimal newTemperature = temperatureSelector.Value;
            string newStatus = $"{newTemperature}°C"; // Формуємо рядок статусу для БД та UI.

            // Формування JSON команди (може містити лише цільову температуру).
            var commandPayload = new { deviceId = selectedDevice.Id, temperature = newTemperature };
            string jsonPayload = JsonSerializer.Serialize(commandPayload);

            // Гібридне шифрування.
            var encryptedPackage = cryptoService.EncryptHybrid(jsonPayload);
            if (encryptedPackage == null)
            {
                Debug.WriteLine("Помилка гібридного шифрування!");
                MessageBox.Show("Помилка шифрування команди.", "Критична помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            UpdateEncryptionTimeLabel(cryptoService.LastEncryptionTimeMs);
            UpdatePerformanceLabels();

            // Формування фінального повідомлення для MQTT.
            var finalPayloadContainer = new
            {
                key = encryptedPackage.Value.encryptedSessionKey,
                iv = encryptedPackage.Value.iv,
                payload = encryptedPackage.Value.encryptedData
            };
            string finalPayload = JsonSerializer.Serialize(finalPayloadContainer);
            string topic = $"smarthome/devices/{selectedDevice.Id}/command_hybrid"; // Той самий топік для команд.

            // Публікація.
            await mqttService.PublishAsync(topic, finalPayload);

            // Оновлення локально.
            dbService.UpdateDeviceStatus(selectedDevice.Id, newStatus);
            selectedDevice.Status = newStatus;
            UpdateDeviceDetails();

            Debug.WriteLine($"MQTT PUBLISH (HYBRID - Thermostat): Topic='{topic}'");
            Debug.WriteLine($"---> Original Payload: {jsonPayload}");
            dbService.LogSecurityEvent("Temperature Set (Hybrid)", $"Device '{selectedDevice.Name}' temperature set to '{newStatus}'. Hybrid encrypted command sent.", loggedInUsername);
        }

        /// <summary>
        /// Обробляє закриття форми (коректне відключення від MQTT).
        /// </summary>
        private async void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            await mqttService.DisconnectAsync();
            mqttService.Dispose();
        }

        /// <summary>
        /// Виконує вихід користувача з системи.
        /// </summary>
        private async void Logout()
        {
            if (loggedInUsername != null)
                dbService.LogSecurityEvent("Logout", "User logged out.", loggedInUsername);
            // Скидання стану.
            loggedInUsername = null!;
            selectedDevice = null!;
            await mqttService.DisconnectAsync();
            ShowLoginView(); // Повернення на екран логіну.
        }

        /// <summary>
        /// Перемикає видимість символів у полі пароля.
        /// </summary>
        private void TogglePasswordButton_Click(object? sender, EventArgs e)
        {
            if (passwordTextBox == null || togglePasswordButton == null) return;

            if (passwordTextBox.PasswordChar == '•')
            {
                passwordTextBox.PasswordChar = '\0'; // Показати.
                togglePasswordButton.Text = "🔒";    // Змінити іконку.
            }
            else
            {
                passwordTextBox.PasswordChar = '•'; // Приховати.
                togglePasswordButton.Text = "👁️";    // Змінити іконку.
            }
        }

        /// <summary>
        /// Обмежує введення у текстові поля (логін, пароль) лише дозволеними символами.
        /// </summary>
        private void InputValidation_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // Дозволено: керуючі символи (Backspace), цифри, англ. літери, @._-
            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar) &&
                !char.IsLetter(e.KeyChar) &&
                e.KeyChar != '@' && e.KeyChar != '.' && e.KeyChar != '_' && e.KeyChar != '-')
            {
                e.Handled = true; // Ігноруємо недозволений символ.
            }
            // Додаткова перевірка, що літери саме англійські (ASCII).
            else if (char.IsLetter(e.KeyChar))
            {
                if (!((e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar >= 'A' && e.KeyChar <= 'Z')))
                {
                    e.Handled = true; // Ігноруємо не-англійські літери.
                }
            }
        }

        /// <summary>
        /// Імітує отримання зашифрованої відповіді (статусу) від вибраного пристрою.
        /// </summary>
        private async void SimulateResponseButton_Click(object? sender, EventArgs e)
        {
            if (selectedDevice == null) return;

            string simulatedStatus = "";
            object statusPayloadJson;

            // Генеруємо фейковий статус залежно від типу пристрою.
            if (selectedDevice.DeviceType == "Лампа")
            {
                simulatedStatus = selectedDevice.Status == "Ввімкнено" ? "Вимкнено" : "Ввімкнено";
                statusPayloadJson = new { deviceId = selectedDevice.Id, status = simulatedStatus };
            }
            else if (selectedDevice.DeviceType == "Термостат")
            {
                // Імітуємо зміну температури на +1.
                if (decimal.TryParse(selectedDevice.Status.Replace("°C", "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal currentTemp))
                {
                    simulatedStatus = $"{currentTemp + 1}°C";
                }
                else
                {
                    simulatedStatus = "20°C"; // Аварійне значення.
                }
                // Відповідь від пристрою зазвичай містить поле 'status'.
                statusPayloadJson = new { deviceId = selectedDevice.Id, status = simulatedStatus };
            }
            else
            {
                Debug.WriteLine("Симуляція відповіді для цього типу пристрою не реалізована.");
                return;
            }

            string jsonPayload = JsonSerializer.Serialize(statusPayloadJson);

            // Шифруємо фейкову відповідь.
            var encryptedPackage = cryptoService.EncryptHybrid(jsonPayload);
            if (encryptedPackage == null)
            {
                Debug.WriteLine("Помилка симуляції шифрування відповіді!");
                return;
            }

            // Формуємо MQTT повідомлення.
            var finalPayloadContainer = new
            {
                key = encryptedPackage.Value.encryptedSessionKey,
                iv = encryptedPackage.Value.iv,
                payload = encryptedPackage.Value.encryptedData
            };
            string finalPayload = JsonSerializer.Serialize(finalPayloadContainer);

            // Створюємо фіктивні аргументи події MQTT.
            var message = new MqttApplicationMessageBuilder()
                .WithTopic($"smarthome/devices/{selectedDevice.Id}/status_hybrid") // Топік статусу.
                .WithPayload(Encoding.UTF8.GetBytes(finalPayload))
                .Build();

            var eventArgs = new MqttApplicationMessageReceivedEventArgs(
                 clientId: "simulated_device", // Вигаданий ID.
                 message,
                 null, // Не використовується в v3.
                 null  // Не використовується тут.
            );

            Debug.WriteLine($"--- SIMULATING MQTT RECEIVE ---");
            // Викликаємо обробник отримання повідомлення напряму.
            await MqttService_MessageReceived(eventArgs);
        }


        #endregion

        #region Обробники подій MQTT

        /// <summary>
        /// Оновлює статус-бар при зміні стану підключення до MQTT брокера.
        /// </summary>
        private void MqttService_ConnectionStateChanged(string state)
        {
            if (statusBar == null || statusLabel == null) return;

            // Використовуємо Invoke для безпечного оновлення UI з іншого потоку.
            if (statusBar.InvokeRequired)
            {
                statusBar.Invoke(new Action(() => statusLabel.Text = state));
            }
            else
            {
                statusLabel.Text = state;
            }
        }

        /// <summary>
        /// Обробляє отримане MQTT повідомлення: розшифровує та оновлює статус пристрою в UI.
        /// </summary>
        private async Task MqttService_MessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            var payloadBytes = e.ApplicationMessage.Payload;
            var payloadString = Encoding.UTF8.GetString(payloadBytes);

            Debug.WriteLine($"MQTT RECEIVE: Topic='{topic}', HybridPayload='{payloadString}'");

            try
            {
                // Розбір JSON контейнера.
                var container = JsonSerializer.Deserialize<JsonElement>(payloadString);
                var encryptedKey = container.GetProperty("key").GetString();
                var iv = container.GetProperty("iv").GetString();
                var encryptedPayload = container.GetProperty("payload").GetString();

                // Перевірка на null перед дешифруванням.
                if (encryptedKey != null && iv != null && encryptedPayload != null)
                {
                    // Гібридне дешифрування.
                    string? decryptedJson = cryptoService.DecryptHybrid(encryptedKey, iv, encryptedPayload);
                    // Оновлення статистики.
                    UpdateDecryptionTimeLabel(cryptoService.LastDecryptionTimeMs);
                    UpdatePerformanceLabels();

                    if (decryptedJson != null)
                    {
                        Debug.WriteLine($"---> Decrypted Payload: {decryptedJson}");
                        var data = JsonSerializer.Deserialize<JsonElement>(decryptedJson);

                        // Витягнення ID пристрою та статусу з розшифрованих даних.
                        if (data.TryGetProperty("deviceId", out var deviceIdElement) && deviceIdElement.TryGetInt32(out int deviceId))
                        {
                            string? status = null;
                            if (data.TryGetProperty("status", out var statusElement) && statusElement.ValueKind == JsonValueKind.String)
                            {
                                status = statusElement.GetString();
                            }

                            if (status != null)
                            {
                                // Оновлення статусу в БД та UI.
                                dbService.UpdateDeviceStatus(deviceId, status);
                                UpdateDeviceStatusInUI(deviceId, status);
                            }
                            else
                            {
                                Debug.WriteLine("Received MQTT message without a valid 'status' field.");
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Received MQTT message without a valid 'deviceId' field.");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Error processing MQTT message: Payload components are null.");
                }
            }
            catch (Exception ex) // Обробка можливих помилок JSON або дешифрування.
            {
                Debug.WriteLine($"Error processing received MQTT message: {ex.Message}");
            }

            await Task.CompletedTask; // Потрібно для async Task обробника.
        }

        #endregion

        #region Завантаження даних та оновлення UI

        /// <summary>
        /// Завантажує список кімнат та пристроїв з БД і відображає їх у TreeView.
        /// </summary>
        private void LoadRoomsAndDevices()
        {
            if (roomsTreeView == null) return;
            roomsTreeView.Nodes.Clear(); // Очищення перед заповненням.
            var rooms = dbService.GetRooms();
            foreach (var room in rooms)
            {
                var roomNode = new TreeNode(room.Name) { Tag = room }; // Зберігаємо об'єкт Room в Tag.
                var devices = dbService.GetDevicesInRoom(room.Id);
                foreach (var device in devices)
                {
                    var deviceNode = new TreeNode(device.Name) { Tag = device }; // Зберігаємо об'єкт Device в Tag.
                    roomNode.Nodes.Add(deviceNode);
                }
                roomsTreeView.Nodes.Add(roomNode);
            }
            roomsTreeView.ExpandAll(); // Розгортаємо всі вузли.
        }

        /// <summary>
        /// Оновлює панель деталей пристрою на основі `selectedDevice`.
        /// </summary>
        private void UpdateDeviceDetails()
        {
            if (selectedDevice == null || deviceNameLabel == null || deviceStatusLabel == null || toggleDeviceButton == null || temperatureSelector == null || deviceDetailsGroupBox == null) return;

            deviceNameLabel.Text = selectedDevice.Name;
            deviceStatusLabel.Text = $"Статус: {selectedDevice.Status}";

            // Спеціальна логіка для термостата: оновлення NumericUpDown.
            if (selectedDevice.DeviceType == "Термостат")
            {
                // Спроба витягти числове значення температури зі статусу.
                if (decimal.TryParse(selectedDevice.Status.Replace("°C", "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tempValue))
                {
                    // Встановлення значення з урахуванням мін/макс NumericUpDown.
                    temperatureSelector.Value = Math.Clamp(tempValue, temperatureSelector.Minimum, temperatureSelector.Maximum);
                }
                else
                {
                    temperatureSelector.Value = temperatureSelector.Minimum; // Або інше значення за замовчуванням.
                }
            }
            deviceDetailsGroupBox.Visible = true; // Робимо панель видимою.
        }


        /// <summary>
        /// Оновлює статус пристрою в UI (TreeView та панель деталей) після отримання повідомлення.
        /// Запускає візуальний ефект підсвічування.
        /// </summary>
        private void UpdateDeviceStatusInUI(int deviceId, string? newStatus)
        {
            if (newStatus == null || roomsTreeView == null) return;

            // Перевірка, чи потрібно викликати метод через Invoke (якщо виклик з іншого потоку).
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateDeviceStatusInUI(deviceId, newStatus)));
                return;
            }

            // Пошук відповідного вузла в TreeView.
            foreach (TreeNode roomNode in roomsTreeView.Nodes)
            {
                foreach (TreeNode deviceNode in roomNode.Nodes)
                {
                    if (deviceNode.Tag is Device device && device.Id == deviceId)
                    {
                        device.Status = newStatus; // Оновлюємо статус в об'єкті Device.
                                                   // Якщо саме цей пристрій зараз вибрано, оновлюємо панель деталей.
                        if (selectedDevice != null && selectedDevice.Id == deviceId)
                        {
                            UpdateDeviceDetails(); // Оновлює і лейбл, і NumericUpDown (якщо треба).
                            HighlightStatusUpdate(); // Запускаємо підсвічування.
                        }
                        // Логуємо подію отримання оновлення.
                        if (loggedInUsername != null)
                            dbService.LogSecurityEvent("Status Update Received", $"Received hybrid encrypted status update for '{device.Name}' to '{newStatus}'.", "System");
                        return; // Пристрій знайдено та оновлено.
                    }
                }
            }
        }

        /// <summary>
        /// Завантажує логи безпеки з БД та відображає їх у DataGridView.
        /// </summary>
        private void LoadSecurityLogs()
        {
            if (logsGridView == null) return;

            var logs = dbService.GetSecurityLogs();
            logsGridView.DataSource = logs; // Встановлюємо джерело даних.
                                            // Налаштування вигляду колонок (якщо вони існують).
            if (logsGridView.Columns.Contains("Id"))
            {
                logsGridView.Columns["Id"].Visible = false; // Сховуємо ID.
                logsGridView.Columns["Timestamp"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss"; // Формат дати/часу.
                logsGridView.Columns["EventType"].HeaderText = "Тип події";
                logsGridView.Columns["Description"].HeaderText = "Опис";
                logsGridView.Columns["Username"].HeaderText = "Користувач";
            }
        }

        /// <summary>
        /// Оновлює текст лейблу часу шифрування у статус-барі.
        /// </summary>
        private void UpdateEncryptionTimeLabel(double timeMs)
        {
            if (statusBar == null || encryptionTimeLabel == null) return;

            string text = $"Ост. шифр.: {timeMs:F4} мс";
            if (statusBar.InvokeRequired)
            {
                statusBar.Invoke(new Action(() => encryptionTimeLabel.Text = text));
            }
            else
            {
                encryptionTimeLabel.Text = text;
            }
        }

        /// <summary>
        /// Оновлює текст лейблу часу дешифрування у статус-барі.
        /// </summary>
        private void UpdateDecryptionTimeLabel(double timeMs)
        {
            if (statusBar == null || decryptionTimeLabel == null) return;

            string text = $"Ост. дешифр.: {timeMs:F4} мс";
            if (statusBar.InvokeRequired)
            {
                statusBar.Invoke(new Action(() => decryptionTimeLabel.Text = text));
            }
            else
            {
                decryptionTimeLabel.Text = text;
            }
        }

        /// <summary>
        /// Оновлює всі лейбли та списки на панелі статистики продуктивності.
        /// </summary>
        private void UpdatePerformanceLabels()
        {
            if (performancePanel == null || avgEncryptionLabel == null || avgDecryptionLabel == null || lastEncryptionLabel == null || lastDecryptionLabel == null || encryptionHistoryListBox == null || decryptionHistoryListBox == null) return;

            if (performancePanel.InvokeRequired)
            {
                performancePanel.Invoke(new Action(UpdatePerformanceLabels));
                return;
            }

            // Оновлення лейблів з останніми та середніми значеннями.
            avgEncryptionLabel.Text = $"Середній час шифрування (AES+RSA): {cryptoService.AverageEncryptionTimeMs:F4} мс";
            avgDecryptionLabel.Text = $"Середній час дешифрування (AES+RSA): {cryptoService.AverageDecryptionTimeMs:F4} мс";
            lastEncryptionLabel.Text = $"Останнє шифрування: {cryptoService.LastEncryptionTimeMs:F4} мс";
            lastDecryptionLabel.Text = $"Останнє дешифрування: {cryptoService.LastDecryptionTimeMs:F4} мс";

            // Оновлення списків історії (найновіші зверху).
            encryptionHistoryListBox.Items.Clear();
            cryptoService.EncryptionTimeHistory.Reverse<double>().ToList().ForEach(t => encryptionHistoryListBox.Items.Add($"{t:F4} мс"));

            decryptionHistoryListBox.Items.Clear();
            cryptoService.DecryptionTimeHistory.Reverse<double>().ToList().ForEach(t => decryptionHistoryListBox.Items.Add($"{t:F4} мс"));
        }

        /// <summary>
        /// Запускає візуальний ефект підсвічування кнопки симуляції.
        /// </summary>
        private void HighlightStatusUpdate()
        {
            if (simulateResponseButton != null && statusHighlightTimer != null)
            {
                simulateResponseButton.BackColor = Color.LightGreen;
                statusHighlightTimer.Start(); // Запускаємо таймер для скидання кольору.
            }
        }


        #endregion

        #region Допоміжні методи

        /// <summary>
        /// Генерує та відображає (у MessageBox та Debug) поточний TOTP код для тестування.
        /// </summary>
        private void GenerateAndDisplayNewTotpCode()
        {
            if (loggedInUsername == null) return;
            string currentCode = mfaService.GenerateCode(loggedInUsername);
            Debug.WriteLine($"--- ДЛЯ ТЕСТУВАННЯ --- Поточний код 2FA: {currentCode} ---");
            MessageBox.Show($"Код для 2FA (для тестування): {currentCode}", "Тестовий код", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Перевіряє наявність користувача 'admin' у БД при запуску програми.
        /// Якщо його немає, створює його з паролем 'password'.
        /// </summary>
        private void CheckAndCreateDefaultUser()
        {
            try
            {
                if (dbService.GetUserByUsername("admin") == null)
                {
                    // Хешуємо пароль 'password'.
                    var (hash, salt) = securityService.HashPassword("password");
                    string hashString = Convert.ToBase64String(hash);
                    string saltString = Convert.ToBase64String(salt);
                    // Додаємо користувача в БД.
                    dbService.AddDefaultUser("admin", hashString, saltString);
                    Debug.WriteLine("Default admin user created successfully.");
                }
            }
            catch (Exception ex) // Обробка можливих помилок БД.
            {
                Debug.WriteLine($"Failed to create or check default user: {ex.Message}");
                MessageBox.Show($"Критична помилка бази даних: {ex.Message}", "Помилка БД", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Можливо, тут варто закрити додаток або перейти в аварійний режим.
            }
        }

        #endregion

    }
}