using System.ComponentModel.DataAnnotations;

namespace Blog10.Models
{
    public class AppSetting
    {
        [Key]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Value { get; set; } = string.Empty;

        public bool IsEncrypted { get; set; } = true;
    }
}