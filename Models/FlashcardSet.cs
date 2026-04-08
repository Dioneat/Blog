namespace Blog10.Models
{
    public class FlashcardSet
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public string CardsJson { get; set; } = "[]"; 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
