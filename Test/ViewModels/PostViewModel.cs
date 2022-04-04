using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Test.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        //public DateTime Date { get; set; } = DateTime.Now;
        public string Date { get { return DateTime.Now.ToString("g"); } }

        public IFormFile File { get; set; }
        public string ImageName { get; set; }
        public string Content { get; set; }
        public string ShortDesc { get; set; }
        public string[] EditTags { get; set; }
        public string PostTags { get; set; }
        public List<string> Tags { get; set; }
    }
}
