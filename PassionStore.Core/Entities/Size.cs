using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class Size : BaseEntity
    {
        public required string Name { get; set; }

        // Navigation property
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = [];
    }

}