using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class CartItemConfiguration : BaseEntityConfiguration<CartItem>
    {
        public override void Configure(EntityTypeBuilder<CartItem> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Cart)
                   .WithMany(x => x.CartItems)
                   .HasForeignKey(e => e.CartId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ProductVariant)
                   .WithMany(x => x.CartItems)
                   .HasForeignKey(e => e.ProductVariantId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}