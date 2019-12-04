using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using MTest.Models.Search;

namespace MTest.Services.Search
{
    public class GoogleSearchService : Abstraction.AbstractSearchService
    {
        protected override string engineName => "google";

        private Google.Apis.Customsearch.v1.CustomsearchService service;

        public GoogleSearchService()
        {
            service = new Google.Apis.Customsearch.v1.CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyDLn7r_DCb6HewQBhvOshC0htdHDyV2Rwg"
            });
        }

        protected override async Task<IEnumerable<SearchResult>> GetResults(string query)
        {
            var result = new List<SearchResult>();

            var request = service.Cse.List(query);
            request.Cx = "004762094579864221485:30x4idfuk6m";

            var response = await request.ExecuteAsync();

            int i = 0;
            foreach (var respItem in response.Items)
            {
                result.Add(new SearchResult()
                {
                    Name = respItem.Title,
                    Description = respItem.Snippet,
                    Link = respItem.Link,
                    Position = i
                });
                i++;
            }

            return result;
        }
    }
}
