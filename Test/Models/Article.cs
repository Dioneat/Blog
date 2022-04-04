using System;

namespace Test.Models
{
    public class Article : TextBlock
    {
        public string Title { get; set; }

        public string ShortDesc { get; set; }


        public string Author { get; set; }

        public string Date { get { return DateTime.Now.ToString("g"); } }

        //[NotMapped]
        //public IFormFile ImageFile { get; set; }
        public string ImageName { get; set; }

        //[NotMapped]
        //public string[] EditTags { get; set; }
        public string Tags { get; set; }




    }
}
