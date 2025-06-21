using PassionStore.Application.DTOs.Brands;
using PassionStore.Application.DTOs.Categories;
using PassionStore.Application.DTOs.ProductVariants;

namespace PassionStore.Application.DTOs.Products
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
        public bool InStock { get; set; } = true;
        public int StockQuantity { get; set; } = 0;
        public double AverageRating { get; set; } = 0.0;
        public bool IsFeatured { get; set; } = false;
        public bool IsSale { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public bool IsNotHadVariants { get; set; } = false;
        public int TotalReviews { get; set; } = 0;
        public int DiscountPercentage { get; set; } = 0;
        public List<ProductImageResponse> ProductImages { get; set; } = [];
        public List<CategoryResponse> Categories { get; set; } = [];
        public List<ProductVariantResponse> ProductVariants { get; set; } = [];
        public required BrandResponse Brand { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

}