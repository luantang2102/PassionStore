using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class SizeConfiguration : BaseEntityConfiguration<Size>

    {
        public override void Configure(EntityTypeBuilder<Size> builder)
        {
            base.Configure(builder);
        }
    }
}