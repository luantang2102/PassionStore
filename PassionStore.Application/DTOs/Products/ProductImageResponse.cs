namespace PassionStore.Application.DTOs.Products
{
    public class ProductImageResponse
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public int Order { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
