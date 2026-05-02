using System.ComponentModel.DataAnnotations;

namespace Blog10.Models
{
    public class SystemLog
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(20)] 
        public string Level { get; set; } = "Info";

        [Required]
        [MaxLength(100)] // "VK Sync", "Telegram API", "System"
        public string Source { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        public string? ExceptionDetails { get; set; }
    }
}