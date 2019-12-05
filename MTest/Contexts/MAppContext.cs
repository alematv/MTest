using Microsoft.EntityFrameworkCore;
using MTest.Models.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTest.Contexts
{
    public class MAppContext : DbContext
    {
        public virtual DbSet<SearchQueryResult> SearchQueryResults { get; set; }

        public virtual DbSet<SearchResult> SearchResults { get; set; }

        public MAppContext(DbContextOptions<MAppContext> opts) : base(opts) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Models.Search.SearchQueryResultConfigurator());
            modelBuilder.ApplyConfiguration(new Models.Search.SearchResultConfigurator());
        }
    }
}
