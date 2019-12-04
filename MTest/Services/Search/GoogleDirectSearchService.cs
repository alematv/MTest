using System;
using System.Collections.Generic;
using AngleSharp.Html.Dom;
using MTest.Models.Search;

namespace MTest.Services.Search
{
    public class GoogleDirectSearchService : Abstraction.AbstractDirectSearchService
    {
        protected override string searchEngineName => "google";

        protected override IEnumerable<SearchResult> GetSearches(IHtmlDocument doc)
        {
            var rcs = doc.GetElementsByClassName("rc");

            return new List<SearchResult>();
        }

        protected override string GetUrlFromQuery(string query)
        {
            return $"https://www.google.com/search?q={query}";
        }
    }
}
