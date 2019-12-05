using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using MTest.Models.Search;

namespace MTest.Services.Search.Abstraction
{
    public abstract class AbstractSearchService : ISearchService
    {
        protected abstract string engineName { get; }
        private Stopwatch stopwatch = new Stopwatch();

        protected abstract Task<IEnumerable<SearchResult>> GetResults(string query);

        public async virtual Task<SearchQueryResult> Query(string query)
        {
            stopwatch.Start();

            var res = new SearchQueryResult() { EngineName = engineName, Query = query, Results = (await GetResults(query)).Take(10).ToList() };

            stopwatch.Stop();
            res.Time = stopwatch.ElapsedMilliseconds;
            res.TimeTaken = DateTime.Now;

            return res;
        }
    }
}
