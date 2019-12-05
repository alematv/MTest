using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTest.Models.Search
{
    public class SearchResultConfigurator : Abstraction.EntityConfigurator<SearchResult>
    {
        protected override string TableName => "search_results";

        protected override void ConfigureColumns(EntityTypeBuilder<SearchResult> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired();

            builder.Property(s => s.Link)
                .IsRequired();

            builder.Property(s => s.Description)
                .IsRequired(false);

            builder.Property(s => s.Position)
                .IsRequired(false);
        }

        protected override void ConfigureIndexes(EntityTypeBuilder<SearchResult> builder)
        {
            builder.HasIndex(s => s.Name);
        }
    }
}
