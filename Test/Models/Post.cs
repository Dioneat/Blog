using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models
{
    public class Post : TextBlock
    {
        public string Title { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string ShortDesc { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        [NotMapped]
        public string[] EditTags { get; set; }
        public string Tags { get; set; }


    }
}
