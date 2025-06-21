using Microsoft.AspNetCore.Http;

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

        // Default variant properties
        public bool IsNotHadVariants { get; set; } = false;
        public decimal DefaultVariantPrice { get; set; } 
        public int DefaultVariantStockQuantity { get; set; } 
    }

}