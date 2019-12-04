using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MTest.Models;
using Google.Apis.Customsearch;
using MTest.Services.Search.Abstraction;
using MTest.Services.Search;
using MTest.Models.Search;

namespace MTest.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search([FromServices] IEnumerable<ISearchService> searchServices, [FromQuery] string q)
        {
            var tasks = new List<Task<SearchQueryResult>>();
            foreach (var service in searchServices)
            {
                tasks.Add(service.Query(q));
            }

            var result = await tasks.ElementAt(Task.WaitAny(tasks.ToArray()));

            return Json(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
