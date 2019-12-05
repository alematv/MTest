using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTest.Models.Search
{
    public class SearchQueryResultConfigurator : Abstraction.EntityConfigurator<SearchQueryResult>
    {
        protected override string TableName => "search_queries";

        protected override void ConfigureColumns(EntityTypeBuilder<SearchQueryResult> builder)
        {
            builder.HasKey(q => q.Id);

            builder.Property(q => q.EngineName)
                .IsRequired();

            builder.Property(q => q.Query)
                .IsRequired();

            builder.Property(q => q.Time)
                .IsRequired();

            builder.Property(q => q.TimeTaken)
                .IsRequired();
        }

        protected override void ConfigureIndexes(EntityTypeBuilder<SearchQueryResult> builder)
        {
            builder.HasIndex(q => q.Query);
        }

        protected override void ConfigureConstraints(EntityTypeBuilder<SearchQueryResult> builder)
        {
            builder.HasMany(q => q.Results)
                .WithOne(sr => sr.SearchQueryResult)
                .HasForeignKey(sr => sr.SearchQueryResultId)
                .IsRequired();
        }
    }
}
