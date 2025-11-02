using System;

namespace Smart_Home_Project
{
    /// <summary>
    /// Представляє запис у лозі безпеки (дані з таблиці SecurityLogs).
    /// </summary>
    public class SecurityLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } // Час виникнення події.
        public string EventType { get; set; } = string.Empty; // Тип події (напр., "Login Failed", "Device Toggled").
        public string Description { get; set; } = string.Empty; // Додатковий опис події.
        public string Username { get; set; } = string.Empty; // Користувач, пов'язаний з подією (може бути порожнім).
    }
}