using PassionStore.Core.Models;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Entities
{
    public class Product : BaseEntity
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public bool InStock { get; set; } = true;
        public double AverageRating { get; set; } = 0.0;
        public bool IsFeatured { get; set; } = false;
        public bool IsSale { get; set; } = false;
        public bool IsNotHadVariants { get; set; } = false;

        // Foreign key
        public Guid BrandId { get; set; }

        // Navigation properties
        public virtual Brand Brand { get; set; } = null!;
        public virtual ICollection<Category> Categories { get; set; } = [];
        public virtual ICollection<ProductImage> ProductImages { get; set; } = [];
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = [];
        public virtual ICollection<Rating> Ratings { get; set; } = [];
    }

}