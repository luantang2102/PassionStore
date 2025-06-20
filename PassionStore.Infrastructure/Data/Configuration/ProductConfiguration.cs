using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Entities;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class ProductConfiguration : BaseEntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            builder.HasOne(p => p.Brand)
                   .WithMany(b => b.Products)
                   .HasForeignKey(p => p.BrandId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Categories)
                   .WithMany(c => c.Products)
                   .UsingEntity(j => j.ToTable("ProductCategories"));
        }
    }

}