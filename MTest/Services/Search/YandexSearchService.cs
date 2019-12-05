using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using MTest.Models.Search;

namespace MTest.Services.Search
{
    public class YandexSearchService : Abstraction.AbstractDirectSearchService
    {
        public YandexSearchService(HttpClient client) : base(client)
        {
        }

        protected override string searchEngineName => "yandex";

        protected override IEnumerable<SearchResult> GetSearches(IHtmlDocument doc)
        {
            var result = new List<SearchResult>();

            var organic = doc.GetElementsByClassName("organic");
            var i = 0;
            foreach (var org in organic)
            {
                var title = org.GetElementsByClassName("organic__url-text").FirstOrDefault()?.TextContent;
                var snippet = org.GetElementsByClassName("organic__content-wrapper").FirstOrDefault();

                var desc = "";
                if (snippet != null)
                {
                    desc = snippet.GetElementsByClassName("extended-text__short").Length == 0 ?
                                    snippet.GetElementsByClassName("organic__text").FirstOrDefault().TextContent :
                                    snippet.GetElementsByClassName("extended-text__short").FirstOrDefault().TextContent;
                }
                var link = org.GetElementsByClassName("organic__url").FirstOrDefault().GetAttribute("href");

                result.Add(new SearchResult()
                {
                    Link = link,
                    Name = title,
                    Description = desc,
                    Position = i
                });
                i++;
            }

            return result;
        }

        protected override string GetUrlFromQuery(string query)
        {
            return $"https://yandex.ru/search/?text={query}&lr=2";
        }
    }
}
