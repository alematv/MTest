using System;
namespace MTest.Models.Search
{
    public class SearchResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public int? Position { get; set; }

        public int SearchQueryResultId { get; set; }
        public SearchQueryResult SearchQueryResult { get; set; }
    }
}
