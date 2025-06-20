namespace PassionStore.Application.DTOs.ProductVariants
{
    public class ProductVariantImageResponse
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
