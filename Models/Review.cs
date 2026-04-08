namespace Blog10.Models
{

    public class Review
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public string AuthorName { get; set; } = string.Empty;

        public string ChildInfo { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDraft { get; set; } = false;
    }
}
