using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Test.ViewModels
{
    public class FilterViewModel
    {
        public FilterViewModel(HashSet<string> tags, string tag, string name)
        {
            Tags = new SelectList(tags, "Id", "Name", tag);
            SelectedTag = tag;
            SelectedName = name;
        }
        public SelectList Tags { get; private set; } 
        public string SelectedTag { get; private set; }   
        public string SelectedName { get; private set; }   
    }
}
