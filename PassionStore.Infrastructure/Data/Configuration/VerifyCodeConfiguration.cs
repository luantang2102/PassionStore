using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class VerifyCodeConfiguration : BaseEntityConfiguration<VerifyCode>
    {
        public override void Configure(EntityTypeBuilder<VerifyCode> builder)
        {
            base.Configure(builder);

            builder.HasOne(v => v.User)
                   .WithMany(u => u.VerifyCodes) // Placeholder; should be u => u.VerifyCodes if added
                   .HasForeignKey(v => v.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}