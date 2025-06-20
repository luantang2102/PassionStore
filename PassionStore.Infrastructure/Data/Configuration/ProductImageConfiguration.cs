using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class ProductImageConfiguration : BaseEntityConfiguration<ProductImage>
    {
        public override void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Product)
                   .WithMany(x => x.ProductImages)
                   .HasForeignKey(e => e.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
