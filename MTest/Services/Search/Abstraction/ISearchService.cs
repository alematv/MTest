using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MTest.Models.Search;

namespace MTest.Services.Search.Abstraction
{
    public interface ISearchService
    {
        Task<SearchQueryResult> Query(string query);
    }
}
