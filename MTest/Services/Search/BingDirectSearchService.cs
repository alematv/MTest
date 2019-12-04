using System;
using System.Collections.Generic;
using AngleSharp.Html.Dom;
using MTest.Models.Search;

namespace MTest.Services.Search
{
    public class BingDirectSearchService : Abstraction.AbstractDirectSearchService
    {
        protected override string searchEngineName => "bing";

        protected override IEnumerable<SearchResult> GetSearches(IHtmlDocument doc)
        {
            return new List<SearchResult>();
        }

        protected override string GetUrlFromQuery(string query)
        {
            return $"https://www.bing.com/search?q={query}";
        }
    }
}
