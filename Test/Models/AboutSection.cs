using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models
{
    public class AboutSection : TextBlock
    {

        public string Images { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public string[] EditImages { get; set; }
        [NotMapped]
        public string[] EditDescription { get; set; }
        [NotMapped]
        public List<IFormFile> FileImages { get; set; }
    }
}
