using System.Collections.Generic;
using Test.Models;

namespace Test.ViewModels
{
    public class SearchViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public HashSet<string> Tags { get; set; }
        public string Categories { get; set; }
        public bool isSearch { get; set; }
    }
}
