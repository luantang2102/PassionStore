using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class Color : BaseEntity
    {
        public required string Name { get; set; }
        public required string HexCode { get; set; }

        // Navigation property
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = [];
    }

}