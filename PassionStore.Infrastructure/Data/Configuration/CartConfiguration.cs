using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class CartConfiguration : BaseEntityConfiguration<Cart>
    {
        public override void Configure(EntityTypeBuilder<Cart> builder)
        {
            base.Configure(builder);

            builder.HasOne(c => c.User)
                   .WithOne(u => u.Cart)
                   .HasForeignKey<Cart>(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
