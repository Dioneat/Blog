namespace Blog10.Models
{
    public class FlashcardModel
    {
        public string IconClass { get; set; } = "fa-solid fa-star";
        public string FrontTitle { get; set; } = "";
        public string BackTitle { get; set; } = "Как выполнять?";
        public string BackDescription { get; set; } = "";
        public bool IsFlipped { get; set; } = false;
    }
    public class QuizResult
    {
        public int MinScore { get; set; }
        public int MaxScore { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    public class QuizModel
    {
        public string Title { get; set; } = "Проверьте себя";
        public string Subtitle { get; set; } = "Пройдите короткий тест";
        public List<QuizQuestion> Questions { get; set; } = new();
    }

    public class QuizQuestion
    {
        public string Text { get; set; } = "";
        public List<QuizAnswer> Answers { get; set; } = new();
    }

    public class QuizAnswer
    {
        public string Text { get; set; } = "";
        public int Score { get; set; }
    }
}