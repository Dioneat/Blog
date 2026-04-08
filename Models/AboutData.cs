namespace Blog10.Models
{
    public class AboutData
    {
        public int Id { get; set; }

        public string HeroTitle { get; set; } = "Елена Тимофеева";
        public string HeroSubtitle { get; set; } = "Логопед-педагог высшей категории";
        public string ProfileImageUrl { get; set; } = "/img/me.jpg";
        public string MainHeroImageUrl { get; set; } = "https://images.unsplash.com/photo-1544717305-2782549b5136?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=774&q=80";

        public string StatsExperience { get; set; } = "10+";
        public string StatsChildren { get; set; } = "500+";
        public string StatsCertificates { get; set; } = "30+";

        public string QuoteText { get; set; } = "Речь — это инструмент, с помощью которого мы открываем мир.";
        public string QuoteAuthor { get; set; } = "Елена Тимофеева";
        public string PrinciplesHtml { get; set; } = "<p>Индивидуальный подход и игровая форма.</p>";

        public string BioHtml { get; set; } = "";
        public string EducationHtml { get; set; } = "";
        public string SkillsHtml { get; set; } = "";
    }
}