using System.ComponentModel.DataAnnotations;

namespace Blog10.Models
{
    public class AdminAccount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)] 
        public string Password { get; set; } = string.Empty;
    }
}