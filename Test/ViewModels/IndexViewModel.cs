using System.Collections.Generic;
using Test.Models;

namespace Test.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public HashSet<string> Tags { get; set; }
    }
}
