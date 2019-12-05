using MTest.Models.Search;
using MTest.Services.Search.Abstraction;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTest.Tests
{
    public class AbstractSearchServiceTests
    {
        private class TestService : AbstractSearchService
        {
            protected override string engineName => "testEngine";

            public bool GetResultsCalled { get; private set; }

            protected override Task<IEnumerable<SearchResult>> GetResults(string query)
            {
                GetResultsCalled = true;
                return Task.FromResult(new List<SearchResult>().AsEnumerable());
            }
        } 

        [Test]
        public async Task Result_must_have_filled_fields()
        {
            var srv = new TestService();
            var res = await srv.Query("test");

            Assert.IsNotNull(res.Results);
            Assert.IsNotNull(res.Time);
            Assert.IsNotNull(res.EngineName);
            Assert.IsNotNull(res.TimeTaken);
        }

        [Test]
        public async Task Result_query_fields_must_be_valid()
        {
            var srv = new TestService();
            var res = await srv.Query("test");

            Assert.AreEqual("test", res.Query);
            Assert.AreEqual("testEngine", res.EngineName);
        }

        [Test]
        public async Task Get_results_method_called()
        {
            var srv = new TestService();
            var res = await srv.Query("test");

            Assert.IsTrue(srv.GetResultsCalled);
        }
    }
}
