using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Entities;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class OrderItemConfiguration : BaseEntityConfiguration<OrderItem>
    {
        public override void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Order)
                   .WithMany(x => x.OrderItems)
                   .HasForeignKey(e => e.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ProductVariant)
                   .WithMany(x => x.OrderItems)
                   .HasForeignKey(e => e.ProductVariantId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}