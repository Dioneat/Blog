using System.ComponentModel.DataAnnotations;

namespace Blog10.Models
{
    public class FlashcardSet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string CardsJson { get; set; } = "[]";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}