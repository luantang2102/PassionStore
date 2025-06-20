using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class ColorConfiguration : BaseEntityConfiguration<Color>

    {
        public override void Configure(EntityTypeBuilder<Color> builder)
        {
            base.Configure(builder);
        }
    }
}