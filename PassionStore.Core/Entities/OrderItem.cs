using PassionStore.Core.Models;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Entities
{
    public class OrderItem : BaseEntity
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Foreign keys
        public Guid ProductVariantId { get; set; } // Updated to link to ProductVariant
        public Guid OrderId { get; set; }

        // Navigation properties
        public virtual ProductVariant ProductVariant { get; set; } = null!; // Updated to link to ProductVariant
        public virtual Order Order { get; set; } = null!;
    }

}