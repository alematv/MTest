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
using MTest.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MTest.Controllers
{
    public class SearchController : Controller
    {
        private MAppContext ctx;

        public SearchController(MAppContext ctx)
        {
            this.ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cached()
        {
            return View();
        }

        public async Task<IActionResult> CachedSearch([FromQuery] string q)
        {
            var result = await ctx.SearchResults
                                .Include(sr => sr.SearchQueryResult)
                                .Where(sr => sr.Name.Contains(q) || sr.Description.Contains(q))
                                .OrderByDescending(sr => sr.SearchQueryResult.TimeTaken)
                                .ToListAsync();

            return Json(result);
        }

        public async Task<IActionResult> Search([FromServices] IEnumerable<ISearchService> searchServices, [FromQuery] string q)
        {
            var tasks = new List<Task<SearchQueryResult>>();
            foreach (var service in searchServices)
            {
                tasks.Add(service.Query(q));
            }

            var result = await tasks.ElementAt(Task.WaitAny(tasks.ToArray()));

            var sameSearch = ctx.SearchQueryResults
                                .Include(sq => sq.Results)
                                .FirstOrDefault(sq => sq.Query == result.Query && sq.EngineName == result.EngineName);

            if (sameSearch != null)
            {
                sameSearch.Results = result.Results;
                sameSearch.EngineName = result.EngineName;
                sameSearch.Time = result.Time;
                sameSearch.TimeTaken = result.TimeTaken;
                ctx.Update(sameSearch);
            }
            else
            {
                await ctx.SearchQueryResults.AddAsync(result);
            }

            await ctx.SaveChangesAsync();

            return Json(result);
        }
    }
}
