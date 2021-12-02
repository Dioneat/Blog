using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Models;

namespace Test.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<IndexSection> IndexSection { get; set; }
        public IEnumerable<Article> Articles { get; set; }

    }
}
