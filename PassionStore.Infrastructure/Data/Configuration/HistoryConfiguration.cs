using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class HistoryConfiguration : BaseEntityConfiguration<History>
    {
        public override void Configure(EntityTypeBuilder<History> builder)
        {
            base.Configure(builder);

            builder.HasOne(h => h.User)
                   .WithMany(u => u.Histories)
                   .HasForeignKey(h => h.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}