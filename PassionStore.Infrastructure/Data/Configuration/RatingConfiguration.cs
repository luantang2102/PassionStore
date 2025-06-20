﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Entities;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class RatingConfiguration : BaseEntityConfiguration<Rating>
    {
        public override void Configure(EntityTypeBuilder<Rating> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.User)
                   .WithMany(x => x.Ratings)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
                   .WithMany(x => x.Ratings)
                   .HasForeignKey(e => e.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
