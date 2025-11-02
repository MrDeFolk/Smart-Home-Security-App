namespace Smart_Home_Project
{
    /// <summary>
    /// Представляє користувача системи (дані з таблиці Users).
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Хеш пароля (Base64).
        public string PasswordSalt { get; set; } = string.Empty; // Сіль для хешування (Base64).
    }
}