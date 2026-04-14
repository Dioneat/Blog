using System.ComponentModel.DataAnnotations;

namespace Blog10.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(2000)]
        public string? Text { get; set; }

        [Required(ErrorMessage = "Имя автора обязательно")]
        [MaxLength(100)]
        public string AuthorName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? ChildInfo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDraft { get; set; } = false;
    }
}