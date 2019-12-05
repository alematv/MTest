using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MTest.Models.Search;

namespace MTest.Services.Search
{
    public class GoogleSearchService : Abstraction.AbstractSearchService
    {
        protected override string engineName => "google";

        private Google.Apis.Customsearch.v1.CustomsearchService service;
        private string CxId;

        public GoogleSearchService(IConfiguration conf)
        {
            service = new Google.Apis.Customsearch.v1.CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                ApiKey = conf.GetValue<string>("SearchServices:Google:ApiKey")
            });
            CxId = conf.GetValue<string>("SearchServices:Google:Cx");
        }

        protected override async Task<IEnumerable<SearchResult>> GetResults(string query)
        {
            var result = new List<SearchResult>();

            var request = service.Cse.List(query);
            request.Cx = CxId;

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
