using System;
using System.Collections.Generic;

namespace MTest.Models.Search
{
    public class SearchQueryResult
    {
        public string EngineName { get; set; }
        public string Query { get; set; }
        public IEnumerable<SearchResult> Results { get; set; }
        public long Time { get; set; }
    }
}
