using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class BrandConfiguration : BaseEntityConfiguration<Brand>

    {
        public override void Configure(EntityTypeBuilder<Brand> builder)
        {
            base.Configure(builder);
        }
    }
}