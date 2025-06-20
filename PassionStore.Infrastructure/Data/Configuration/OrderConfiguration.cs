using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class OrderConfiguration : BaseEntityConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.UserProfile)
                   .WithMany(x => x.Orders)
                   .HasForeignKey(e => e.UserProfileId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ShippingAddress)
                   .WithMany(x => x.Orders)
                   .HasForeignKey(e => e.ShippingAddressId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
