using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog10.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Заголовок обязателен")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string CoverImageUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [MaxLength(1000)]
        public string ShortDescription { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new List<string>();

        public bool IsDraft { get; set; } = true;

        [MaxLength(250)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? FileAttachmentUrl { get; set; }

        [MaxLength(250)]
        public string? FileAttachmentName { get; set; }

        [MaxLength(500)]
        public string? MainAudioUrl { get; set; }

        [MaxLength(500)]
        public string? MainVideoUrl { get; set; }

        public int? QuizId { get; set; }
        public int? FlashcardSetId { get; set; }
        public long? VkPostId { get; set; }

        [ForeignKey("QuizId")]
        public Quiz? Quiz { get; set; }

        [ForeignKey("FlashcardSetId")]
        public FlashcardSet? FlashcardSet { get; set; }

        [NotMapped]
        public int ReadTime
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Content)) return 1;
                int wordCount = Content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
                int time = wordCount / 180;
                return time < 1 ? 1 : time;
            }
        }
    }
}