using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class ProductVariantImageConfiguration : BaseEntityConfiguration<ProductVariantImage>
    {
        public override void Configure(EntityTypeBuilder<ProductVariantImage> builder)
        {
            base.Configure(builder);

            builder.HasOne(p => p.ProductVariant)
                   .WithMany(p => p.ProductVariantImages)
                   .HasForeignKey(p => p.ProductVariantId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}