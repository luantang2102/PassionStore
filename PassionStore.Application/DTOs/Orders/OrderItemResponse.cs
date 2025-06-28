using PassionStore.Application.DTOs.Colors;
using PassionStore.Application.DTOs.Sizes;

namespace PassionStore.Application.DTOs.Orders
{
    public class OrderItemResponse
    {
        public Guid Id { get; set; }

        // Product
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;

        // Product Variant
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public required ColorResponse Color { get; set; }
        public required SizeResponse Size { get; set; }
    }
}
