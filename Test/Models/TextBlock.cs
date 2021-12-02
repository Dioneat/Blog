using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Models
{
    public abstract class TextBlock
    {

        public int Id { get; set; }
        public string Content { get; set; }
    }
}
