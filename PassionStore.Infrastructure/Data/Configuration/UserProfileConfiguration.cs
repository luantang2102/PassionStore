using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class UserProfileConfiguration : BaseEntityConfiguration<UserProfile>
    {
        public override void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.User)
                   .WithMany(x => x.UserProfiles)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Address)
                   .WithMany(x => x.UserProfiles)
                   .HasForeignKey(e => e.AddressId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}