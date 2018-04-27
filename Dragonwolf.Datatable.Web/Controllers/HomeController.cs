using Dragonwolf.Datatable.Web.Models;
using Dragonwolf.Razor.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Dragonwolf.Datatable.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new HomeModel() { ListeItems = new List<ItemModel>() };

            var randomizer = new Random((int)DateTime.Now.ToFileTimeUtc());

            var nombreValeurs = 10 * randomizer.Next(1000);

            for (var i = 0; i < nombreValeurs; i++)
            {
                model.ListeItems.Add(new ItemModel()
                {
                    Id = i,
                    Libelle = "Test " + i,
                    Test = "Test " + (nombreValeurs - i),
                    Test2 = i.ToString(),
                    Test3 = nombreValeurs / (i == 0 ? 1 : i),
                    Test4 = (i % 2 == 0)
                });
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public JsonResult GetTestDatas(string textSearch = null, PagingSortingFilterModel pagingSortingFilter = null)
        {
            JsonResult result = null;

            var tempResult = new PagingSortingFilterResult<ItemModel>();

            var randomizer = new Random((int)DateTime.Now.ToFileTimeUtc());

            var nombreValeurs = 10 * randomizer.Next(1000);

            for (var i = 0; i < nombreValeurs; i++)
            {
                tempResult.Results.Add(new ItemModel()
                {
                    Id = i,
                    Libelle = "Test " + i,
                    Test = "Test " + (nombreValeurs - i),
                    Test2 = i.ToString(),
                    Test3 = nombreValeurs / (i == 0 ? 1 : i),
                    Test4 = (i % 2 == 0)
                });
            }

            tempResult.TotalResultats = tempResult.Results.Count;

            if (pagingSortingFilter != null)
            {
                if (pagingSortingFilter.Sorting != null && pagingSortingFilter.Sorting.Any(s => !string.IsNullOrEmpty(s.Key)))
                {
                    var sortinglist = pagingSortingFilter.Sorting.Where(s => !string.IsNullOrEmpty(s.Key)).ToList();

                    IOrderedEnumerable<ItemModel> tempResultQuery = null;
                    var properties = typeof(ItemModel).GetProperties();

                    foreach (var sort in sortinglist)
                    {
                        var property = properties.FirstOrDefault(p => p.Name.Equals(sort.Key, StringComparison.InvariantCultureIgnoreCase));

                        if (sortinglist.IndexOf(sort) == 0)
                        {
                            if (sort.Ascendant)
                            {
                                tempResultQuery = tempResult.Results.OrderBy(o => property.GetValue(o));
                            }
                            else
                            {
                                tempResultQuery = tempResult.Results.OrderByDescending(o => property.GetValue(o));
                            }
                        }
                        else
                        {
                            if (sort.Ascendant)
                            {
                                tempResultQuery = tempResultQuery.ThenBy(o => property.GetValue(o));
                            }
                            else
                            {
                                tempResultQuery = tempResultQuery.ThenByDescending(o => property.GetValue(o));
                            }
                        }
                    }

                    tempResult.Results = tempResultQuery.ToList();
                }

                if (pagingSortingFilter.Paging != null)
                {
                    tempResult.Page = pagingSortingFilter.Paging.Page;
                    tempResult.NombreResultatsParPage = pagingSortingFilter.Paging.ResultatsParPage;

                    var minimalIndex = pagingSortingFilter.Paging.Page * pagingSortingFilter.Paging.ResultatsParPage;

                    tempResult.Results = tempResult.Results.Skip(minimalIndex).Take(pagingSortingFilter.Paging.ResultatsParPage).ToList();
                }
            }

            result = Json(tempResult, JsonRequestBehavior.AllowGet);

            return result;
        }
    }
}