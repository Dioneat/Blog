using System.ComponentModel.DataAnnotations;

namespace Blog10.Models
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Subtitle { get; set; } = string.Empty;

        public string QuestionsJson { get; set; } = "[]";
        public string ResultsJson { get; set; } = "[]";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}