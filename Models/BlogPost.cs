using System.ComponentModel.DataAnnotations.Schema;

namespace Blog10.Models
{
    public class BlogPost
    {
        public int Id { get; set; } 
        public string Title { get; set; } = string.Empty; 
        public string CoverImageUrl { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
        public string ShortDescription { get; set; } = string.Empty; 

        public string Content { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new List<string>();

        public bool IsDraft { get; set; } = true;
        public string Slug { get; set; } = string.Empty;

        public string? FileAttachmentUrl { get; set; }
        public string? FileAttachmentName { get; set; } 


        public string? MainAudioUrl { get; set; }
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

                int wordCount = Content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
                int time = wordCount / 180; 

                return time < 1 ? 1 : time;
            }
        }
    }
}
