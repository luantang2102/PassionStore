using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class CartItem : BaseEntity
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Foreign keys
        public Guid ProductVariantId { get; set; } // Updated to link to ProductVariant
        public Guid CartId { get; set; }

        // Navigation properties
        public virtual ProductVariant ProductVariant { get; set; } = null!; // Updated to link to ProductVariant
        public virtual Cart Cart { get; set; } = null!;
    }

}