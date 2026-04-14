using System.ComponentModel.DataAnnotations;

namespace Blog10.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Название категории слишком длинное")]
        public string Name { get; set; } = string.Empty;
    }
}