using System.ComponentModel.DataAnnotations;

namespace Blog10.Models
{
    public class SystemLog
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Тип: Info, Warning, Error
        public string Level { get; set; } = "Info";

        // "VK Sync", "Telegram", "System"
        public string Source { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? ExceptionDetails { get; set; }
    }
}