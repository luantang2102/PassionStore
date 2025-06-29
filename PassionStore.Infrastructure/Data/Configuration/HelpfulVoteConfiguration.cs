using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Entities;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class HelpfulVoteConfiguration : IEntityTypeConfiguration<HelpfulVote>
    {
        public void Configure(EntityTypeBuilder<HelpfulVote> builder)
        {
            builder.HasKey(hv => hv.Id);

            builder.Property(hv => hv.Id)
                .ValueGeneratedOnAdd();

            builder.Property(hv => hv.CreatedDate)
                .IsRequired();

            builder.HasOne(hv => hv.Rating)
                .WithMany(r => r.HelpfulVotes)
                .HasForeignKey(hv => hv.RatingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(hv => hv.User)
                .WithMany(u => u.HelpfulVotes)
                .HasForeignKey(hv => hv.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(hv => new { hv.RatingId, hv.UserId })
                .IsUnique(); // Ensure one vote per user per rating
        }
    }
}