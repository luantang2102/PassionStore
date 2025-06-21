using PassionStore.Core.Entities;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class ProductVariant : BaseEntity
    {
        public decimal Price { get; set; }
        public int StockQuantity { get; set; } = 0;

        // Foreign keys
        public Guid ProductId { get; set; }
        public Guid ColorId { get; set; }
        public Guid SizeId { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; } = null!;
        public virtual Color Color { get; set; } = null!;
        public virtual Size Size { get; set; } = null!;
        public virtual ICollection<CartItem> CartItems { get; set; } = [];
        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];
    }

}