namespace Smart_Home_Project
{
    /// <summary>
    /// Представляє пристрій розумного будинку (дані з таблиці Devices).
    /// </summary>
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Ініціалізація порожнім рядком.
        public string Status { get; set; } = string.Empty; // Поточний статус пристрою (напр., "Ввімкнено", "22°C").
        public string DeviceType { get; set; } = string.Empty; // Тип пристрою (напр., "Лампа", "Термостат").
                                                               // RoomId та TypeId не зберігаємо тут, оскільки вони потрібні лише для зв'язку в БД.
    }
}