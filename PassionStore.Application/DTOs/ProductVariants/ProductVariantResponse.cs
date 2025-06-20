using PassionStore.Application.DTOs.Colors;
using PassionStore.Application.DTOs.Sizes;

namespace PassionStore.Application.DTOs.ProductVariants
{
    public class ProductVariantResponse
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public List<ProductVariantImageResponse> Images { get; set; } = [];
        public required SizeResponse Size { get; set; }
        public required ColorResponse Color { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
