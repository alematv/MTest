using AngleSharp.Html.Dom;
using Moq;
using Moq.Protected;
using MTest.Models.Search;
using MTest.Services.Search.Abstraction;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTest.Tests
{
    public class AbstractDirectSearchServiceTests
    {
        private HttpClient client;
        private HttpClient timeoutClient;
        private HttpClient unsuccessfullClient;

        [SetUp]
        public void Setup()
        {
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("[]")
               });

            client = new HttpClient(handler.Object);

            var timeoutHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            timeoutHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .Throws(new TimeoutException("Service timeout"));

            timeoutClient = new HttpClient(timeoutHandler.Object);

            var unsuccessfullHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            unsuccessfullHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.NotFound,
                   Content = new StringContent("[]")
               });

            unsuccessfullClient = new HttpClient(unsuccessfullHandler.Object);
        }


        private class TestService : AbstractDirectSearchService
        {
            public bool GetSearchesCalled { get; private set; }
            public bool GetUrlFromQueryCalled { get; private set; }

            public TestService(HttpClient client) : base(client)
            {
            }

            protected override string searchEngineName => "test";

            protected override IEnumerable<SearchResult> GetSearches(IHtmlDocument doc)
            {
                GetSearchesCalled = true;
                return new List<SearchResult>() { new SearchResult() };
            }

            protected override string GetUrlFromQuery(string query)
            {
                GetUrlFromQueryCalled = true;
                return $"http://query";
            }
        }

        [Test]
        public async Task Service_get_proper_name()
        {
            var service = new TestService(client);
            var res = await service.Query("test");

            Assert.AreEqual("test-direct", res.EngineName);
        }

        [Test]
        public async Task Overrides_called()
        {
            var service = new TestService(client);
            var res = await service.Query("test");

            Assert.IsTrue(service.GetSearchesCalled);
            Assert.IsTrue(service.GetUrlFromQueryCalled);
        }

        [Test]
        public async Task Does_not_throws_on_time_out()
        {
            var service = new TestService(timeoutClient);
            SearchQueryResult res = null;

            Assert.DoesNotThrowAsync(async () => { res = await service.Query("test"); });
            Assert.IsNotNull(res.Results);
            Assert.AreEqual(0, res.Results.Count);
        }

        [Test]
        public async Task Return_empty_list_on_unsuccesfull_response()
        {
            var service = new TestService(unsuccessfullClient);
            var res = await service.Query("test"); ;

            Assert.IsNotNull(res.Results);
            Assert.AreEqual(0, res.Results.Count);
        }
    }
}
