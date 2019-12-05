using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using MTest.Models.Search;

namespace MTest.Services.Search.Abstraction
{
    public abstract class AbstractDirectSearchService : AbstractSearchService
    {
        protected HttpClient client;
        protected HtmlParser parser;

        protected override string engineName => $"{searchEngineName}-direct";

        protected abstract string searchEngineName { get; }

        public AbstractDirectSearchService(HttpClient client)
        {
            this.client = client;
            parser = new HtmlParser();
        }

        protected async override Task<IEnumerable<SearchResult>> GetResults(string query)
        {
            try
            {
                var response = await client.GetAsync(GetUrlFromQuery(query));

                if (response.IsSuccessStatusCode)
                {
                    var stringHtml = await response.Content.ReadAsStringAsync();

                    var doc = await parser.ParseDocumentAsync(stringHtml, default);

                    return GetSearches(doc);
                }

                return new List<SearchResult>();
            }
            catch
            {
                return new List<SearchResult>();
            }
        }

        protected abstract string GetUrlFromQuery(string query);
        protected abstract IEnumerable<SearchResult> GetSearches(IHtmlDocument doc);
    }
}
