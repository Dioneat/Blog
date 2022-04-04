using System.Collections.Generic;
using Test.Models;

namespace Test.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<IndexSection> IndexSection { get; set; }
        public IEnumerable<Article> Articles { get; set; }

    }
}
