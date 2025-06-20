using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class ReportConfiguration : BaseEntityConfiguration<Report>
    {
        public override void Configure(EntityTypeBuilder<Report> builder)
        {
            base.Configure(builder);

            builder.HasOne(r => r.Sender)
                   .WithMany(u => u.Reports)
                   .HasForeignKey(r => r.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}