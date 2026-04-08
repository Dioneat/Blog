namespace Blog10.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty; 
        public string Subtitle { get; set; } = string.Empty;
        public string QuestionsJson { get; set; } = "[]"; 
        public string ResultsJson { get; set; } = "[]"; 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
