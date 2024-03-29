﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;

using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.Extensions.Configuration;
using MTest.Models.Search;
using MTest.Services.Search.Abstraction;

namespace MTest.Services.Search
{
    public class BingSearchService : AbstractSearchService
    {
        private WebSearchClient client;

        protected override string engineName => "bing";

        public BingSearchService(IConfiguration conf)
        {
            client = new WebSearchClient(new ApiKeyServiceClientCredentials(conf.GetValue<string>("SearchServices:Bing:ApiKey")));
        }

        protected override async Task<IEnumerable<SearchResult>> GetResults(string query)
        {
            var result = new List<SearchResult>();

            var searchData = await client.Web.SearchAsync(query: query, responseFilter: new List<string>() { "Webpages" });

            if (searchData?.WebPages?.Value?.Count > 0)
            {
                int i = 0;
                foreach (var page in searchData.WebPages.Value)
                {
                    result.Add(new SearchResult()
                    {
                        Name = page.Name,
                        Description = page.Snippet,
                        Link = page.Url,
                        Position = i
                    });
                    i++;
                }
            }

            return result;
        }
    }
}
