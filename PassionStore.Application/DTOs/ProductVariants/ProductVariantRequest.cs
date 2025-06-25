namespace PassionStore.Application.DTOs.ProductVariants
{
    public class ProductVariantRequest
    {
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid ProductId { get; set; }
        public Guid ColorId { get; set; }
        public Guid SizeId { get; set; }
    }
}