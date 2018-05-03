using Dragonwolf.Razor.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonwolf.Datatable.Web.Models
{
    public class HomeModel : BaseModel
    {
        public List<ItemModel> ListeItems { get; set; }
    }
}