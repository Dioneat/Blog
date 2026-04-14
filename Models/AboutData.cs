using System.ComponentModel.DataAnnotations;

namespace Blog10.Models
{
    public class AboutData
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "Тег слишком длинный")]
        public string? HeroTag { get; set; } = "Логопед-педагог • Опыт 12 лет";

        [MaxLength(200)]
        public string? HeroTitle { get; set; } = "Говорим красиво и уверенно!";

        [MaxLength(500)]
        public string? HeroDescription { get; set; } = "Помогаю детям преодолеть речевые трудности через игру и доказательные методики.";

        [MaxLength(500)]
        public string? MainHeroImageUrl { get; set; } = "/img/default-hero.jpg"; 

        [MaxLength(150)]
        public string? AboutSectionTitle { get; set; } = "Мой подход к работе";

        [MaxLength(300)]
        public string? AboutSectionSubtitle { get; set; } = "Эффективное обучение в атмосфере доверия.";

        public string? AboutBlocksJson { get; set; } = "[]";

        [MaxLength(200)]
        public string? HeroSubtitle { get; set; } = "Логопед-педагог высшей категории";

        [MaxLength(500)]
        public string? ProfileImageUrl { get; set; } = "/img/me.jpg";
        [MaxLength(50)]
        public string? StatsExperience { get; set; } = "10+";

        [MaxLength(50)]
        public string? StatsChildren { get; set; } = "500+";

        [MaxLength(50)]
        public string? StatsCertificates { get; set; } = "30+";
        public string? AboutPageTitle { get; set; } = "Елена Тимофеева"; 

        [MaxLength(1000)]
        public string? QuoteText { get; set; } = "Речь — это инструмент, с помощью которого мы открываем мир.";

        [MaxLength(100)]
        public string? QuoteAuthor { get; set; } = "Мыслитель";

        public string? PrinciplesHtml { get; set; } = "<p>Индивидуальный подход и игровая форма.</p>";
        public string? BioHtml { get; set; } = "";
        public string? EducationHtml { get; set; } = "";
        public string? SkillsHtml { get; set; } = "";
    }
}