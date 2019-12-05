using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTest.Models.Abstraction
{
    public abstract class EntityConfigurator<T> : IEntityTypeConfiguration<T> where T : class
    {
        protected abstract string TableName { get; }

        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.ToTable(TableName);

            ConfigureColumns(builder);

            ConfigureIndexes(builder);

            ConfigureConstraints(builder);
        }

        protected virtual void ConfigureConstraints(EntityTypeBuilder<T> builder) { }
        protected virtual void ConfigureIndexes(EntityTypeBuilder<T> builder) { }
        protected abstract void ConfigureColumns(EntityTypeBuilder<T> builder);
    }
}
