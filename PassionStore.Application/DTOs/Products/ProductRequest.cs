using Microsoft.AspNetCore.Http;
using PassionStore.Application.DTOs.ProductVariants;

namespace PassionStore.Application.DTOs.Products
{
    public class ProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool InStock { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public List<ExistingProductImageRequest> Images { get; set; } = [];
        public List<IFormFile> FormImages { get; set; } = [];
        public List<Guid> CategoryIds { get; set; } = [];
        public Guid BrandId { get; set; } = Guid.Empty;
    }
}
