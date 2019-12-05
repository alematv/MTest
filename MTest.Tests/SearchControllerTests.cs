using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using MTest.Contexts;
using MTest.Controllers;
using MTest.Models.Search;
using MTest.Services.Search.Abstraction;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTest.Tests
{
    public class SearchContollerTests
    {
        DbContextOptions<MAppContext> options;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<MAppContext>()
                                                    .UseInMemoryDatabase("testDb", new InMemoryDatabaseRoot())
                                                    .Options;
        }

        [Test]
        public async Task Cached_search_return_valid_result_json()
        {
            using (var context = new MAppContext(options))
            using (var controller = new SearchController(context))
            {
                var result = await controller.CachedSearch("test toast");

                Assert.IsInstanceOf<JsonResult>(result);
                Assert.IsInstanceOf<List<SearchResult>>((result as JsonResult).Value);
            }
        }

        [Test]
        public async Task Cached_search_return_filtered_results()
        {
            var queryData = new List<SearchQueryResult>
            {
                new SearchQueryResult { EngineName = "bing", Query = "test", Time = 656, Results = new List<SearchResult>()
                {
                    new SearchResult() {  Name = "test 1", Description = "test 1", Link = "test1", Position = 0, SearchQueryResultId = 1 },
                    new SearchResult() {  Name = "test 2", Description = "test 2", Link = "test2", Position = 0, SearchQueryResultId = 1 },
                } },
                new SearchQueryResult { EngineName = "google", Query = "test toast", Time = 656, Results = new List<SearchResult>()
                {
                    new SearchResult() {  Name = "test 1", Description = "test 1", Link = "test1", Position = 0, SearchQueryResultId = 1 },
                    new SearchResult() {  Name = "test toast 2", Description = "test toast 2", Link = "test2", Position = 0, SearchQueryResultId = 1 },
                } }
            };

            using (var context = new MAppContext(options))
            {
                await context.SearchQueryResults.AddRangeAsync(queryData);
                await context.SaveChangesAsync();
            }

            using (var context = new MAppContext(options))
            using (var controller = new SearchController(context))
            {
                var result = await controller.CachedSearch("test toast");

                Assume.That(result is JsonResult);
                Assume.That((result as JsonResult).Value is IEnumerable<SearchResult>);

                Assert.AreEqual(1, ((result as JsonResult).Value as IEnumerable<SearchResult>).Count());
            }
        }

        [Test]
        public async Task Search_same_query_via_same_engine_replace_results()
        {
            var query = "tea";
            var oldTimeTaken = DateTime.Now.AddSeconds(-5);
            var newTimeTaken = DateTime.Now;

            var queryData = new List<SearchQueryResult>
            {
                new SearchQueryResult { EngineName = "bing", Query = query, Time = 656, TimeTaken = oldTimeTaken, Results = new List<SearchResult>()
                {
                    new SearchResult() {  Name = "test1", Description = "test1", Link = "test1", Position = 0, SearchQueryResultId = 1 },
                    new SearchResult() {  Name = "test2", Description = "test2", Link = "test2", Position = 0, SearchQueryResultId = 1 },
                } }
            };

            using (var context = new MAppContext(options))
            {
                await context.SearchQueryResults.AddRangeAsync(queryData);
                await context.SaveChangesAsync();
            }

            using (var context = new MAppContext(options))
            using (var controller = new SearchController(context))
            {
                var searchService = new Mock<ISearchService>();
                searchService.Setup(ss => ss.Query(It.IsAny<string>())).ReturnsAsync(new SearchQueryResult
                {
                    EngineName = "bing",
                    Query = query,
                    Time = 225,
                    TimeTaken = newTimeTaken,
                    Results = new List<SearchResult>() {
                        new SearchResult() { Name = "test1replace", Description = "test1replace", Link = "test1replace", Position = 0 },
                        new SearchResult() { Name = "test1replace", Description = "test1replace", Link = "test1replace", Position = 0 },
                        new SearchResult() { Name = "test1replace", Description = "test1replace", Link = "test1replace", Position = 0 }
                    }
                });

                var result = await controller.Search(new List<ISearchService>() { searchService.Object }, query);
            }

            using (var context = new MAppContext(options))
            {
                var searchQueryCount = context.SearchQueryResults.ToList().Count;
                var searchResultCount = context.SearchResults.ToList().Count;
                var searchQuery = context.SearchQueryResults.Find(1);

                Assert.AreEqual("bing", searchQuery.EngineName);
                Assert.AreEqual(225, searchQuery.Time);
                Assert.AreEqual(newTimeTaken, searchQuery.TimeTaken);
                Assert.AreEqual(1, searchQueryCount);
                Assert.AreEqual(3, searchResultCount);
            }
        }

        [Test]
        public async Task Search_same_query_via_diff_engine_add_results()
        {
            var query = "tea";

            var queryData = new List<SearchQueryResult>
            {
                new SearchQueryResult { EngineName = "bing", Query = query, Time = 656, Results = new List<SearchResult>()
                {
                    new SearchResult() {  Name = "test1", Description = "test1", Link = "test1", Position = 0, SearchQueryResultId = 1 },
                    new SearchResult() {  Name = "test2", Description = "test2", Link = "test2", Position = 0, SearchQueryResultId = 1 },
                } }
            };

            using (var context = new MAppContext(options))
            {
                await context.SearchQueryResults.AddRangeAsync(queryData);
                await context.SaveChangesAsync();
            }

            using (var context = new MAppContext(options))
            using (var controller = new SearchController(context))
            {
                var searchService = new Mock<ISearchService>();
                searchService.Setup(ss => ss.Query(It.IsAny<string>())).ReturnsAsync(new SearchQueryResult
                {
                    EngineName = "google",
                    Query = query,
                    Time = 225,
                    Results = new List<SearchResult>() {
                        new SearchResult() { Name = "test1replace", Description = "test1replace", Link = "test1replace", Position = 0 },
                        new SearchResult() { Name = "test1replace", Description = "test1replace", Link = "test1replace", Position = 0 },
                        new SearchResult() { Name = "test1replace", Description = "test1replace", Link = "test1replace", Position = 0 }
                    }
                });

                var result = await controller.Search(new List<ISearchService>() { searchService.Object }, query);
            }

            using (var context = new MAppContext(options))
            {
                var searchQueryCount = context.SearchQueryResults.ToList().Count;
                var searchResultCount = context.SearchResults.ToList().Count;

                Assert.AreEqual(2, searchQueryCount);
                Assert.AreEqual(5, searchResultCount);
            }
        }

        [Test]
        public async Task Search_diff_query_add_results()
        {
            var query = "tea";
            var newQuery = "coffee";

            var queryData = new List<SearchQueryResult>
            {
                new SearchQueryResult { EngineName = "bing", Query = query, Time = 656, Results = new List<SearchResult>()
                {
                    new SearchResult() {  Name = "test1", Description = "test1", Link = "test1", Position = 0, SearchQueryResultId = 1 },
                    new SearchResult() {  Name = "test2", Description = "test2", Link = "test2", Position = 0, SearchQueryResultId = 1 },
                } }
            };

            using (var context = new MAppContext(options))
            {
                await context.SearchQueryResults.AddRangeAsync(queryData);
                await context.SaveChangesAsync();
            }

            using (var context = new MAppContext(options))
            using (var controller = new SearchController(context))
            {
                var searchService = new Mock<ISearchService>();
                searchService.Setup(ss => ss.Query(It.IsAny<string>())).ReturnsAsync(new SearchQueryResult
                {
                    EngineName = "google",
                    Query = newQuery,
                    Time = 225,
                    Results = new List<SearchResult>() {
                        new SearchResult() { Name = "test3", Description = "test3", Link = "test3", Position = 0 },
                        new SearchResult() { Name = "test4", Description = "test4", Link = "test4", Position = 0 },
                        new SearchResult() { Name = "test5", Description = "test5", Link = "test5", Position = 0 }
                    }
                });

                var result = await controller.Search(new List<ISearchService>() { searchService.Object }, newQuery);
            }

            using (var context = new MAppContext(options))
            {
                var searchQueryCount = context.SearchQueryResults.ToList().Count;
                var searchResultCount = context.SearchResults.ToList().Count;

                Assert.AreEqual(2, searchQueryCount);
                Assert.AreEqual(5, searchResultCount);
            }
        }

        [Test]
        public async Task Late_search_service_result_disposed()
        {
            using (var context = new MAppContext(options))
            using (var controller = new SearchController(context))
            {
                var slowService = new Mock<ISearchService>();
                slowService.Setup(ss => ss.Query(It.IsAny<string>())).Returns(async () => {
                    await Task.Delay(10);
                    return new SearchQueryResult
                            {
                                EngineName = "google",
                                Query = "tea",
                                Time = 10,
                                Results = new List<SearchResult>() {
                                new SearchResult() { Name = "test3", Description = "test3", Link = "test3", Position = 0 },
                                new SearchResult() { Name = "test4", Description = "test4", Link = "test4", Position = 0 },
                                new SearchResult() { Name = "test5", Description = "test5", Link = "test5", Position = 0 }
                            }
                    };
                });
                var fastService = new Mock<ISearchService>();
                fastService.Setup(ss => ss.Query(It.IsAny<string>())).ReturnsAsync(() => {
                    return new SearchQueryResult
                    {
                        EngineName = "yandex",
                        Query = "tea",
                        Time = 5,
                        Results = new List<SearchResult>() {
                                new SearchResult() { Name = "test3", Description = "test3", Link = "test3", Position = 0 },
                                new SearchResult() { Name = "test4", Description = "test4", Link = "test4", Position = 0 },
                                new SearchResult() { Name = "test5", Description = "test5", Link = "test5", Position = 0 }
                            }
                    };
                });

                var result = await controller.Search(new List<ISearchService>() { slowService.Object, fastService.Object }, "tea");
            }

            using (var context = new MAppContext(options))
            {
                var searchQueryCount = (await context.SearchQueryResults.ToListAsync()).Count;
                var searchQuery = await context.SearchQueryResults.FirstOrDefaultAsync();

                Assert.AreEqual(1, searchQueryCount);
                Assert.AreEqual("yandex", searchQuery.EngineName);
            }
        }
    }
}