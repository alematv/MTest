using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MTest.Models.Search
{
    public class SearchQueryResult
    {
        public int Id { get; set; }
        public string EngineName { get; set; }
        public string Query { get; set; }
        public long Time { get; set; }
        [DisplayFormat(DataFormatString = @"{0:dd.MM.yyyy HH:mm:ss}")]
        public DateTime TimeTaken { get; set; }

        public ICollection<SearchResult> Results { get; set; }
    }
}
