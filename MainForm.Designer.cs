namespace Smart_Home_Project
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            loginPanel = new Panel();
            togglePasswordButton = new Button();
            loginButton = new Button();
            passwordTextBox = new TextBox();
            usernameTextBox = new TextBox();
            label2 = new Label();
            label1 = new Label();
            welcomeLabel = new Label();
            menuStrip = new MenuStrip();
            statusBar = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            encryptionTimeLabel = new ToolStripStatusLabel();
            decryptionTimeLabel = new ToolStripStatusLabel();
            totpPanel = new Panel();
            resendTotpButton = new Button();
            confirmTotpButton = new Button();
            totpTextBox = new TextBox();
            label4 = new Label();
            label3 = new Label();
            dashboardPanel = new Panel();
            mainSplitContainer = new SplitContainer();
            roomsTreeView = new TreeView();
            rightPanel = new Panel();
            deviceDetailsGroupBox = new GroupBox();
            simulateResponseButton = new Button();
            setTemperatureButton = new Button();
            tempUnitLabel = new Label();
            temperatureSelector = new NumericUpDown();
            toggleDeviceButton = new Button();
            deviceStatusLabel = new Label();
            deviceNameLabel = new Label();
            logsPanel = new Panel();
            logsGridView = new DataGridView();
            label5 = new Label();
            performancePanel = new Panel();
            decryptionHistoryListBox = new ListBox();
            label9 = new Label();
            encryptionHistoryListBox = new ListBox();
            label8 = new Label();
            lastDecryptionLabel = new Label();
            lastEncryptionLabel = new Label();
            avgDecryptionLabel = new Label();
            avgEncryptionLabel = new Label();
            label7 = new Label();
            aboutProgramPanel = new Panel();
            label12 = new Label();
            label11 = new Label();
            label10 = new Label();
            aboutAuthorPanel = new Panel();
            label15 = new Label();
            label14 = new Label();
            label13 = new Label();
            authorPictureBox = new PictureBox();
            loginPanel.SuspendLayout();
            statusBar.SuspendLayout();
            totpPanel.SuspendLayout();
            dashboardPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
            mainSplitContainer.Panel1.SuspendLayout();
            mainSplitContainer.Panel2.SuspendLayout();
            mainSplitContainer.SuspendLayout();
            rightPanel.SuspendLayout();
            deviceDetailsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)temperatureSelector).BeginInit();
            logsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logsGridView).BeginInit();
            performancePanel.SuspendLayout();
            aboutProgramPanel.SuspendLayout();
            aboutAuthorPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)authorPictureBox).BeginInit();
            SuspendLayout();
            // 
            // loginPanel
            // 
            loginPanel.BackColor = SystemColors.ActiveCaption;
            loginPanel.Controls.Add(togglePasswordButton);
            loginPanel.Controls.Add(loginButton);
            loginPanel.Controls.Add(passwordTextBox);
            loginPanel.Controls.Add(usernameTextBox);
            loginPanel.Controls.Add(label2);
            loginPanel.Controls.Add(label1);
            loginPanel.Controls.Add(welcomeLabel);
            loginPanel.Dock = DockStyle.Fill;
            loginPanel.Location = new Point(0, 24);
            loginPanel.Name = "loginPanel";
            loginPanel.Size = new Size(900, 676);
            loginPanel.TabIndex = 0;
            // 
            // togglePasswordButton
            // 
            togglePasswordButton.Anchor = AnchorStyles.None;
            togglePasswordButton.Location = new Point(580, 301);
            togglePasswordButton.Name = "togglePasswordButton";
            togglePasswordButton.Size = new Size(30, 26);
            togglePasswordButton.TabIndex = 6;
            togglePasswordButton.Text = "👁️";
            togglePasswordButton.UseVisualStyleBackColor = true;
            // 
            // loginButton
            // 
            loginButton.Anchor = AnchorStyles.None;
            loginButton.Location = new Point(400, 346);
            loginButton.Name = "loginButton";
            loginButton.Size = new Size(100, 23);
            loginButton.TabIndex = 5;
            loginButton.Text = "Далі";
            loginButton.UseVisualStyleBackColor = true;
            // 
            // passwordTextBox
            // 
            passwordTextBox.Anchor = AnchorStyles.None;
            passwordTextBox.Location = new Point(375, 301);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '•';
            passwordTextBox.Size = new Size(200, 23);
            passwordTextBox.TabIndex = 4;
            // 
            // usernameTextBox
            // 
            usernameTextBox.Anchor = AnchorStyles.None;
            usernameTextBox.Location = new Point(375, 266);
            usernameTextBox.Name = "usernameTextBox";
            usernameTextBox.Size = new Size(200, 23);
            usernameTextBox.TabIndex = 3;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.None;
            label2.AutoSize = true;
            label2.Location = new Point(300, 304);
            label2.Name = "label2";
            label2.Size = new Size(52, 15);
            label2.TabIndex = 2;
            label2.Text = "Пароль:";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.None;
            label1.AutoSize = true;
            label1.Location = new Point(300, 269);
            label1.Name = "label1";
            label1.Size = new Size(40, 15);
            label1.TabIndex = 1;
            label1.Text = "Логін:";
            // 
            // welcomeLabel
            // 
            welcomeLabel.Anchor = AnchorStyles.None;
            welcomeLabel.AutoSize = true;
            welcomeLabel.Font = new Font("Arial", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            welcomeLabel.Location = new Point(375, 211);
            welcomeLabel.Name = "welcomeLabel";
            welcomeLabel.Size = new Size(146, 26);
            welcomeLabel.TabIndex = 0;
            welcomeLabel.Text = "Авторизація";
            // 
            // menuStrip
            // 
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(900, 24);
            menuStrip.TabIndex = 1;
            menuStrip.Text = "menuStrip1";
            // 
            // statusBar
            // 
            statusBar.Items.AddRange(new ToolStripItem[] { statusLabel, encryptionTimeLabel, decryptionTimeLabel });
            statusBar.Location = new Point(0, 678);
            statusBar.Name = "statusBar";
            statusBar.Size = new Size(900, 22);
            statusBar.TabIndex = 2;
            statusBar.Text = "statusStrip1";
            statusBar.Visible = false;
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(45, 17);
            statusLabel.Text = "Готово";
            // 
            // encryptionTimeLabel
            // 
            encryptionTimeLabel.Name = "encryptionTimeLabel";
            encryptionTimeLabel.Size = new Size(420, 17);
            encryptionTimeLabel.Spring = true;
            encryptionTimeLabel.Text = "Ост. шифр.: - мс";
            encryptionTimeLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // decryptionTimeLabel
            // 
            decryptionTimeLabel.Name = "decryptionTimeLabel";
            decryptionTimeLabel.Size = new Size(420, 17);
            decryptionTimeLabel.Spring = true;
            decryptionTimeLabel.Text = "Ост. дешифр.: - мс";
            decryptionTimeLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // totpPanel
            // 
            totpPanel.BackColor = SystemColors.ActiveCaption;
            totpPanel.Controls.Add(resendTotpButton);
            totpPanel.Controls.Add(confirmTotpButton);
            totpPanel.Controls.Add(totpTextBox);
            totpPanel.Controls.Add(label4);
            totpPanel.Controls.Add(label3);
            totpPanel.Dock = DockStyle.Fill;
            totpPanel.Location = new Point(0, 24);
            totpPanel.Name = "totpPanel";
            totpPanel.Size = new Size(900, 676);
            totpPanel.TabIndex = 3;
            totpPanel.Visible = false;
            // 
            // resendTotpButton
            // 
            resendTotpButton.Anchor = AnchorStyles.None;
            resendTotpButton.FlatAppearance.BorderSize = 0;
            resendTotpButton.FlatStyle = FlatStyle.Flat;
            resendTotpButton.Font = new Font("Arial", 8F, FontStyle.Underline);
            resendTotpButton.ForeColor = Color.Blue;
            resendTotpButton.Location = new Point(390, 371);
            resendTotpButton.Name = "resendTotpButton";
            resendTotpButton.Size = new Size(120, 23);
            resendTotpButton.TabIndex = 4;
            resendTotpButton.Text = "Надіслати знову";
            resendTotpButton.UseVisualStyleBackColor = true;
            // 
            // confirmTotpButton
            // 
            confirmTotpButton.Anchor = AnchorStyles.None;
            confirmTotpButton.Location = new Point(400, 331);
            confirmTotpButton.Name = "confirmTotpButton";
            confirmTotpButton.Size = new Size(100, 23);
            confirmTotpButton.TabIndex = 3;
            confirmTotpButton.Text = "Увійти";
            confirmTotpButton.UseVisualStyleBackColor = true;
            // 
            // totpTextBox
            // 
            totpTextBox.Anchor = AnchorStyles.None;
            totpTextBox.Location = new Point(390, 291);
            totpTextBox.MaxLength = 6;
            totpTextBox.Name = "totpTextBox";
            totpTextBox.Size = new Size(120, 23);
            totpTextBox.TabIndex = 2;
            totpTextBox.TextAlign = HorizontalAlignment.Center;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.None;
            label4.AutoSize = true;
            label4.Location = new Point(335, 261);
            label4.Name = "label4";
            label4.Size = new Size(226, 15);
            label4.TabIndex = 1;
            label4.Text = "Введіть 6-значний код з вашого додатку";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.None;
            label3.AutoSize = true;
            label3.Font = new Font("Arial", 16F, FontStyle.Bold);
            label3.Location = new Point(290, 211);
            label3.Name = "label3";
            label3.Size = new Size(330, 26);
            label3.TabIndex = 0;
            label3.Text = "Двофакторна автентифікація";
            // 
            // dashboardPanel
            // 
            dashboardPanel.Controls.Add(mainSplitContainer);
            dashboardPanel.Dock = DockStyle.Fill;
            dashboardPanel.Location = new Point(0, 24);
            dashboardPanel.Name = "dashboardPanel";
            dashboardPanel.Padding = new Padding(10);
            dashboardPanel.Size = new Size(900, 676);
            dashboardPanel.TabIndex = 4;
            dashboardPanel.Visible = false;
            // 
            // mainSplitContainer
            // 
            mainSplitContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            mainSplitContainer.Location = new Point(10, 10);
            mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel1
            // 
            mainSplitContainer.Panel1.Controls.Add(roomsTreeView);
            // 
            // mainSplitContainer.Panel2
            // 
            mainSplitContainer.Panel2.Controls.Add(rightPanel);
            mainSplitContainer.Size = new Size(880, 656);
            mainSplitContainer.SplitterDistance = 300;
            mainSplitContainer.TabIndex = 0;
            // 
            // roomsTreeView
            // 
            roomsTreeView.Dock = DockStyle.Fill;
            roomsTreeView.Location = new Point(0, 0);
            roomsTreeView.Name = "roomsTreeView";
            roomsTreeView.Size = new Size(300, 656);
            roomsTreeView.TabIndex = 0;
            // 
            // rightPanel
            // 
            rightPanel.Controls.Add(deviceDetailsGroupBox);
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Location = new Point(0, 0);
            rightPanel.Name = "rightPanel";
            rightPanel.Padding = new Padding(10);
            rightPanel.Size = new Size(576, 656);
            rightPanel.TabIndex = 0;
            // 
            // deviceDetailsGroupBox
            // 
            deviceDetailsGroupBox.Controls.Add(simulateResponseButton);
            deviceDetailsGroupBox.Controls.Add(setTemperatureButton);
            deviceDetailsGroupBox.Controls.Add(tempUnitLabel);
            deviceDetailsGroupBox.Controls.Add(temperatureSelector);
            deviceDetailsGroupBox.Controls.Add(toggleDeviceButton);
            deviceDetailsGroupBox.Controls.Add(deviceStatusLabel);
            deviceDetailsGroupBox.Controls.Add(deviceNameLabel);
            deviceDetailsGroupBox.Dock = DockStyle.Fill;
            deviceDetailsGroupBox.Location = new Point(10, 10);
            deviceDetailsGroupBox.Name = "deviceDetailsGroupBox";
            deviceDetailsGroupBox.Size = new Size(556, 636);
            deviceDetailsGroupBox.TabIndex = 0;
            deviceDetailsGroupBox.TabStop = false;
            deviceDetailsGroupBox.Text = "Інформація про пристрій";
            deviceDetailsGroupBox.Visible = false;
            // 
            // simulateResponseButton
            // 
            simulateResponseButton.Anchor = AnchorStyles.Left;
            simulateResponseButton.Enabled = false;
            simulateResponseButton.Location = new Point(20, 151);
            simulateResponseButton.Name = "simulateResponseButton";
            simulateResponseButton.Size = new Size(150, 23);
            simulateResponseButton.TabIndex = 6;
            simulateResponseButton.Text = "Імітувати відповідь";
            simulateResponseButton.UseVisualStyleBackColor = true;
            // 
            // setTemperatureButton
            // 
            setTemperatureButton.Anchor = AnchorStyles.Left;
            setTemperatureButton.Location = new Point(120, 109);
            setTemperatureButton.Name = "setTemperatureButton";
            setTemperatureButton.Size = new Size(100, 23);
            setTemperatureButton.TabIndex = 5;
            setTemperatureButton.Text = "Встановити";
            setTemperatureButton.UseVisualStyleBackColor = true;
            setTemperatureButton.Visible = false;
            // 
            // tempUnitLabel
            // 
            tempUnitLabel.Anchor = AnchorStyles.Left;
            tempUnitLabel.AutoSize = true;
            tempUnitLabel.Location = new Point(85, 113);
            tempUnitLabel.Name = "tempUnitLabel";
            tempUnitLabel.Size = new Size(20, 15);
            tempUnitLabel.TabIndex = 4;
            tempUnitLabel.Text = "°C";
            tempUnitLabel.Visible = false;
            // 
            // temperatureSelector
            // 
            temperatureSelector.Anchor = AnchorStyles.Left;
            temperatureSelector.Location = new Point(20, 111);
            temperatureSelector.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            temperatureSelector.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            temperatureSelector.Name = "temperatureSelector";
            temperatureSelector.Size = new Size(60, 23);
            temperatureSelector.TabIndex = 3;
            temperatureSelector.Value = new decimal(new int[] { 20, 0, 0, 0 });
            temperatureSelector.Visible = false;
            // 
            // toggleDeviceButton
            // 
            toggleDeviceButton.Anchor = AnchorStyles.Left;
            toggleDeviceButton.Location = new Point(20, 111);
            toggleDeviceButton.Name = "toggleDeviceButton";
            toggleDeviceButton.Size = new Size(120, 23);
            toggleDeviceButton.TabIndex = 2;
            toggleDeviceButton.Text = "Переключити";
            toggleDeviceButton.UseVisualStyleBackColor = true;
            toggleDeviceButton.Visible = false;
            // 
            // deviceStatusLabel
            // 
            deviceStatusLabel.AutoSize = true;
            deviceStatusLabel.Location = new Point(20, 60);
            deviceStatusLabel.Name = "deviceStatusLabel";
            deviceStatusLabel.Size = new Size(46, 15);
            deviceStatusLabel.TabIndex = 1;
            deviceStatusLabel.Text = "Статус:";
            // 
            // deviceNameLabel
            // 
            deviceNameLabel.AutoSize = true;
            deviceNameLabel.Font = new Font("Arial", 12F, FontStyle.Bold);
            deviceNameLabel.Location = new Point(20, 30);
            deviceNameLabel.Name = "deviceNameLabel";
            deviceNameLabel.Size = new Size(122, 19);
            deviceNameLabel.TabIndex = 0;
            deviceNameLabel.Text = "Ім'я пристрою";
            // 
            // logsPanel
            // 
            logsPanel.Controls.Add(logsGridView);
            logsPanel.Controls.Add(label5);
            logsPanel.Dock = DockStyle.Fill;
            logsPanel.Location = new Point(0, 24);
            logsPanel.Name = "logsPanel";
            logsPanel.Padding = new Padding(10);
            logsPanel.Size = new Size(900, 676);
            logsPanel.TabIndex = 5;
            logsPanel.Visible = false;
            // 
            // logsGridView
            // 
            logsGridView.AllowUserToAddRows = false;
            logsGridView.AllowUserToDeleteRows = false;
            logsGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            logsGridView.BackgroundColor = Color.White;
            logsGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            logsGridView.Dock = DockStyle.Fill;
            logsGridView.Location = new Point(10, 50);
            logsGridView.Name = "logsGridView";
            logsGridView.ReadOnly = true;
            logsGridView.RowHeadersVisible = false;
            logsGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            logsGridView.Size = new Size(880, 616);
            logsGridView.TabIndex = 1;
            // 
            // label5
            // 
            label5.Dock = DockStyle.Top;
            label5.Font = new Font("Arial", 16F, FontStyle.Bold);
            label5.Location = new Point(10, 10);
            label5.Name = "label5";
            label5.Size = new Size(880, 40);
            label5.TabIndex = 0;
            label5.Text = "Логи безпеки";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // performancePanel
            // 
            performancePanel.Controls.Add(decryptionHistoryListBox);
            performancePanel.Controls.Add(label9);
            performancePanel.Controls.Add(encryptionHistoryListBox);
            performancePanel.Controls.Add(label8);
            performancePanel.Controls.Add(lastDecryptionLabel);
            performancePanel.Controls.Add(lastEncryptionLabel);
            performancePanel.Controls.Add(avgDecryptionLabel);
            performancePanel.Controls.Add(avgEncryptionLabel);
            performancePanel.Controls.Add(label7);
            performancePanel.Dock = DockStyle.Fill;
            performancePanel.Location = new Point(0, 24);
            performancePanel.Name = "performancePanel";
            performancePanel.Padding = new Padding(20);
            performancePanel.Size = new Size(900, 676);
            performancePanel.TabIndex = 6;
            performancePanel.Visible = false;
            // 
            // decryptionHistoryListBox
            // 
            decryptionHistoryListBox.Anchor = AnchorStyles.None;
            decryptionHistoryListBox.FormatString = "F4 мс";
            decryptionHistoryListBox.FormattingEnabled = true;
            decryptionHistoryListBox.ItemHeight = 15;
            decryptionHistoryListBox.Location = new Point(475, 281);
            decryptionHistoryListBox.Name = "decryptionHistoryListBox";
            decryptionHistoryListBox.Size = new Size(250, 154);
            decryptionHistoryListBox.TabIndex = 8;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.None;
            label9.AutoSize = true;
            label9.Location = new Point(475, 261);
            label9.Name = "label9";
            label9.Size = new Size(229, 15);
            label9.TabIndex = 7;
            label9.Text = "Історія часу дешифрування (останні 10):";
            // 
            // encryptionHistoryListBox
            // 
            encryptionHistoryListBox.Anchor = AnchorStyles.None;
            encryptionHistoryListBox.FormatString = "F4 мс";
            encryptionHistoryListBox.FormattingEnabled = true;
            encryptionHistoryListBox.ItemHeight = 15;
            encryptionHistoryListBox.Location = new Point(175, 281);
            encryptionHistoryListBox.Name = "encryptionHistoryListBox";
            encryptionHistoryListBox.Size = new Size(250, 154);
            encryptionHistoryListBox.TabIndex = 6;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.None;
            label8.AutoSize = true;
            label8.Location = new Point(175, 261);
            label8.Name = "label8";
            label8.Size = new Size(217, 15);
            label8.TabIndex = 5;
            label8.Text = "Історія часу шифрування (останні 10):";
            // 
            // lastDecryptionLabel
            // 
            lastDecryptionLabel.Anchor = AnchorStyles.None;
            lastDecryptionLabel.AutoSize = true;
            lastDecryptionLabel.Location = new Point(175, 221);
            lastDecryptionLabel.Name = "lastDecryptionLabel";
            lastDecryptionLabel.Size = new Size(143, 15);
            lastDecryptionLabel.TabIndex = 4;
            lastDecryptionLabel.Text = "Останнє дешифрування:";
            // 
            // lastEncryptionLabel
            // 
            lastEncryptionLabel.Anchor = AnchorStyles.None;
            lastEncryptionLabel.AutoSize = true;
            lastEncryptionLabel.Location = new Point(175, 191);
            lastEncryptionLabel.Name = "lastEncryptionLabel";
            lastEncryptionLabel.Size = new Size(131, 15);
            lastEncryptionLabel.TabIndex = 3;
            lastEncryptionLabel.Text = "Останнє шифрування:";
            // 
            // avgDecryptionLabel
            // 
            avgDecryptionLabel.Anchor = AnchorStyles.None;
            avgDecryptionLabel.AutoSize = true;
            avgDecryptionLabel.Location = new Point(175, 161);
            avgDecryptionLabel.Name = "avgDecryptionLabel";
            avgDecryptionLabel.Size = new Size(229, 15);
            avgDecryptionLabel.TabIndex = 2;
            avgDecryptionLabel.Text = "Середній час дешифрування (AES+RSA):";
            // 
            // avgEncryptionLabel
            // 
            avgEncryptionLabel.Anchor = AnchorStyles.None;
            avgEncryptionLabel.AutoSize = true;
            avgEncryptionLabel.Location = new Point(175, 131);
            avgEncryptionLabel.Name = "avgEncryptionLabel";
            avgEncryptionLabel.Size = new Size(217, 15);
            avgEncryptionLabel.TabIndex = 1;
            avgEncryptionLabel.Text = "Середній час шифрування (AES+RSA):";
            // 
            // label7
            // 
            label7.Dock = DockStyle.Top;
            label7.Font = new Font("Arial", 16F, FontStyle.Bold);
            label7.Location = new Point(20, 20);
            label7.Name = "label7";
            label7.Size = new Size(860, 40);
            label7.TabIndex = 0;
            label7.Text = "Статистика продуктивності криптографії";
            label7.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // aboutProgramPanel
            // 
            aboutProgramPanel.Controls.Add(label12);
            aboutProgramPanel.Controls.Add(label11);
            aboutProgramPanel.Controls.Add(label10);
            aboutProgramPanel.Dock = DockStyle.Fill;
            aboutProgramPanel.Location = new Point(0, 24);
            aboutProgramPanel.Name = "aboutProgramPanel";
            aboutProgramPanel.Padding = new Padding(20);
            aboutProgramPanel.Size = new Size(900, 676);
            aboutProgramPanel.TabIndex = 7;
            aboutProgramPanel.Visible = false;
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.None;
            label12.Location = new Point(158, 261);
            label12.Name = "label12";
            label12.Size = new Size(584, 100);
            label12.TabIndex = 2;
            label12.Text = resources.GetString("label12.Text");
            label12.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.None;
            label11.AutoSize = true;
            label11.Location = new Point(420, 211);
            label11.Name = "label11";
            label11.Size = new Size(60, 15);
            label11.TabIndex = 1;
            label11.Text = "Версія 1.0";
            // 
            // label10
            // 
            label10.Dock = DockStyle.Top;
            label10.Font = new Font("Arial", 16F, FontStyle.Bold);
            label10.Location = new Point(20, 20);
            label10.Name = "label10";
            label10.Size = new Size(860, 40);
            label10.TabIndex = 0;
            label10.Text = "Smart Home Security Control Panel";
            label10.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // aboutAuthorPanel
            // 
            aboutAuthorPanel.Controls.Add(label15);
            aboutAuthorPanel.Controls.Add(label14);
            aboutAuthorPanel.Controls.Add(label13);
            aboutAuthorPanel.Controls.Add(authorPictureBox);
            aboutAuthorPanel.Dock = DockStyle.Fill;
            aboutAuthorPanel.Location = new Point(0, 24);
            aboutAuthorPanel.Name = "aboutAuthorPanel";
            aboutAuthorPanel.Padding = new Padding(20);
            aboutAuthorPanel.Size = new Size(900, 676);
            aboutAuthorPanel.TabIndex = 8;
            aboutAuthorPanel.Visible = false;
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.None;
            label15.AutoSize = true;
            label15.Font = new Font("Microsoft Sans Serif", 10F);
            label15.Location = new Point(379, 421);
            label15.Name = "label15";
            label15.Size = new Size(141, 17);
            label15.TabIndex = 3;
            label15.Text = "Група: КСІМ-24006м";
            // 
            // label14
            // 
            label14.Anchor = AnchorStyles.None;
            label14.AutoSize = true;
            label14.Font = new Font("Microsoft Sans Serif", 10F);
            label14.Location = new Point(379, 397);
            label14.Name = "label14";
            label14.Size = new Size(115, 17);
            label14.TabIndex = 2;
            label14.Text = "Автор: MrDeFolk";
            // 
            // label13
            // 
            label13.Dock = DockStyle.Top;
            label13.Font = new Font("Arial", 16F, FontStyle.Bold);
            label13.Location = new Point(20, 20);
            label13.Name = "label13";
            label13.Size = new Size(860, 40);
            label13.TabIndex = 1;
            label13.Text = "Про автора";
            label13.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // authorPictureBox
            // 
            authorPictureBox.Anchor = AnchorStyles.None;
            authorPictureBox.BackColor = SystemColors.ControlLight;
            authorPictureBox.BorderStyle = BorderStyle.FixedSingle;
            authorPictureBox.Image = Properties.Resources.tetoris;
            authorPictureBox.Location = new Point(375, 191);
            authorPictureBox.Name = "authorPictureBox";
            authorPictureBox.Size = new Size(150, 150);
            authorPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            authorPictureBox.TabIndex = 0;
            authorPictureBox.TabStop = false;
            // 
            // MainForm
            // 
            ClientSize = new Size(900, 700);
            Controls.Add(aboutAuthorPanel);
            Controls.Add(loginPanel);
            Controls.Add(aboutProgramPanel);
            Controls.Add(performancePanel);
            Controls.Add(logsPanel);
            Controls.Add(dashboardPanel);
            Controls.Add(totpPanel);
            Controls.Add(menuStrip);
            Controls.Add(statusBar);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            MinimumSize = new Size(640, 480);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Smart Home Security Control Panel";
            loginPanel.ResumeLayout(false);
            loginPanel.PerformLayout();
            statusBar.ResumeLayout(false);
            statusBar.PerformLayout();
            totpPanel.ResumeLayout(false);
            totpPanel.PerformLayout();
            dashboardPanel.ResumeLayout(false);
            mainSplitContainer.Panel1.ResumeLayout(false);
            mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
            mainSplitContainer.ResumeLayout(false);
            rightPanel.ResumeLayout(false);
            deviceDetailsGroupBox.ResumeLayout(false);
            deviceDetailsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)temperatureSelector).EndInit();
            logsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)logsGridView).EndInit();
            performancePanel.ResumeLayout(false);
            performancePanel.PerformLayout();
            aboutProgramPanel.ResumeLayout(false);
            aboutProgramPanel.PerformLayout();
            aboutAuthorPanel.ResumeLayout(false);
            aboutAuthorPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)authorPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Control Variable Declarations
        // --- Login Panel ---
        private System.Windows.Forms.Panel loginPanel;
        private System.Windows.Forms.Button togglePasswordButton;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label welcomeLabel;

        // --- TOTP Panel ---
        private System.Windows.Forms.Panel totpPanel;
        private System.Windows.Forms.Button resendTotpButton;
        private System.Windows.Forms.Button confirmTotpButton;
        private System.Windows.Forms.TextBox totpTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;

        // --- Dashboard Panel ---
        private System.Windows.Forms.Panel dashboardPanel;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.TreeView roomsTreeView;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.GroupBox deviceDetailsGroupBox;
        private System.Windows.Forms.Button simulateResponseButton;
        private System.Windows.Forms.Button setTemperatureButton;
        private System.Windows.Forms.Label tempUnitLabel;
        private System.Windows.Forms.NumericUpDown temperatureSelector;
        private System.Windows.Forms.Button toggleDeviceButton;
        private System.Windows.Forms.Label deviceStatusLabel;
        private System.Windows.Forms.Label deviceNameLabel;

        // --- Logs Panel ---
        private System.Windows.Forms.Panel logsPanel;
        private System.Windows.Forms.DataGridView logsGridView;
        private System.Windows.Forms.Label label5; // logsLabel

        // --- Performance Panel ---
        private System.Windows.Forms.Panel performancePanel;
        private System.Windows.Forms.ListBox decryptionHistoryListBox;
        private System.Windows.Forms.Label label9; // decHistoryLabel
        private System.Windows.Forms.ListBox encryptionHistoryListBox;
        private System.Windows.Forms.Label label8; // encHistoryLabel
        private System.Windows.Forms.Label lastDecryptionLabel;
        private System.Windows.Forms.Label lastEncryptionLabel;
        private System.Windows.Forms.Label avgDecryptionLabel;
        private System.Windows.Forms.Label avgEncryptionLabel;
        private System.Windows.Forms.Label label7; // titleLabel

        // --- About Program Panel ---
        private System.Windows.Forms.Panel aboutProgramPanel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;

        // --- About Author Panel ---
        private System.Windows.Forms.Panel aboutAuthorPanel;
        private System.Windows.Forms.PictureBox authorPictureBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;


        // --- Main Controls ---
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel encryptionTimeLabel;
        private System.Windows.Forms.ToolStripStatusLabel decryptionTimeLabel;

        #endregion

    }
}