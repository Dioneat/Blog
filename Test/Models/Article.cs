using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Models
{
    public class Article : TextBlock
    {
        public string Title { get; set; }

        public string ShortDesc { get; set; }


        public string Author { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public string ImageName { get; set; }

        [NotMapped]
        public string[] EditTags { get; set; }
        public string Tags { get; set; }




    }
}
