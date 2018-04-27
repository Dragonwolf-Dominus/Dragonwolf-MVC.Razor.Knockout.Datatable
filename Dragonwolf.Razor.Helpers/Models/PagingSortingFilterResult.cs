using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonwolf.Razor.Helpers.Models
{
    public class PagingSortingFilterResult<T>
    {
        #region Properties
        public List<T> Results { get; set; }

        public int Page { get; set; }

        public int NombreResultatsParPage { get; set; }

        public int TotalResultats { get; set; }
        #endregion

        #region Constructor
        public PagingSortingFilterResult()
        {
            this.Results = new List<T>();
        }
        #endregion
    }       
}