using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class ChatConfiguration : BaseEntityConfiguration<Chat>
    {
        public override void Configure(EntityTypeBuilder<Chat> builder)
        {
            base.Configure(builder);

            builder.HasOne(c => c.User)
                   .WithMany(u => u.Chats)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}