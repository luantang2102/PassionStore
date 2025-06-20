using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class ProductVariantConfiguration : BaseEntityConfiguration<ProductVariant>
    {
        public override void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            base.Configure(builder);

            builder.HasOne(p => p.Product)
                   .WithMany(p => p.ProductVariants)
                   .HasForeignKey(p => p.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Color)
                   .WithMany(c => c.ProductVariants)
                   .HasForeignKey(p => p.ColorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Size)
                   .WithMany(s => s.ProductVariants)
                   .HasForeignKey(p => p.SizeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}