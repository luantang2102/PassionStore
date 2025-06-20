using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class ProductVariantImage : BaseEntity
    {
        public required string ImageUrl { get; set; }
        public required string PublicId { get; set; }
        public bool IsMain { get; set; }

        // Foreign key
        public Guid ProductVariantId { get; set; }

        // Navigation property
        public virtual ProductVariant ProductVariant { get; set; } = null!;
    }

}