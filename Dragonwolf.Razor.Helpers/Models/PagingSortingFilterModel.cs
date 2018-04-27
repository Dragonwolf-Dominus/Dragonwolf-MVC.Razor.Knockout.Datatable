using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonwolf.Razor.Helpers.Models
{
    public class PagingSortingFilterModel
    {
        public Paging Paging { get; set; }

        public List<Sorting> Sorting { get; set; }
    }

    public class Paging
    {
        public int Page { get; set; }

        public int ResultatsParPage { get; set; }
    }

    public class Sorting
    {
        public string Key { get; set; }

        public bool Ascendant { get; set; }
    }
}

