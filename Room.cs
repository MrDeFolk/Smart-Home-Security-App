namespace Smart_Home_Project
{
    /// <summary>
    /// Представляє кімнату в будинку (дані з таблиці Rooms).
    /// </summary>
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Ініціалізація порожнім рядком.
    }
}