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
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index([FromServices] IEnumerable<ISearchService> searchServices)
        {

            var tasks = new List<Task<SearchQueryResult>>();
            foreach (var service in searchServices)
            {
                tasks.Add(service.Query("coffee"));
            }


            var result = await tasks.ElementAt(Task.WaitAny(tasks.ToArray()));

            return View(result);
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
