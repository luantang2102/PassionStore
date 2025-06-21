using PassionStore.Application.DTOs.Products;
using PassionStore.Application.DTOs.ProductVariants;
using PassionStore.Core.Entities;
using PassionStore.Core.Models;

namespace PassionStore.Application.Mappers
{
    public static class ProductMapper
    {
        public static ProductResponse MapModelToResponse(this Product product)
        {
            var averageRating = product.Ratings.Count > 0 ? product.Ratings.Average(x => x.Value) : 0;
            var minPrice = product.ProductVariants.Count > 0 ? product.ProductVariants.Min(x => x.Price) : 0;
            var maxPrice = product.ProductVariants.Count > 0 ? product.ProductVariants.Max(x => x.Price) : 0;
            var stockQuantity = product.ProductVariants.Sum(x => x.StockQuantity);
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                InStock = product.InStock,
                AverageRating = averageRating,
                IsFeatured = product.IsFeatured,
                ProductImages = product.ProductImages.Select(pi => pi.MapModelToResponse()).OrderBy(pi => pi.Order).ToList(),
                Categories = product.Categories.Select(c => c.MapModelToResponse()).ToList(),
                ProductVariants = product.ProductVariants.Select(pv => pv.MapModelToResponse()).ToList(),
                Brand = product.Brand.MapModelToResponse(),
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate,
                MaxPrice = maxPrice,
                MinPrice = minPrice,
                StockQuantity = stockQuantity,
                IsNotHadVariants = product.IsNotHadVariants,
                //IsSale = product.IsSale,
                //IsNew = product.IsNew,
                TotalReviews = product.Ratings.Count,
                //DiscountPercentage = product.DiscountPercentage
            };
        }

        public static ProductImageResponse MapModelToResponse(this ProductImage productImage)
        {
            return new ProductImageResponse
            {
                Id = productImage.Id,
                ImageUrl = productImage.ImageUrl,
                PublicId = productImage.PublicId,
                Order = productImage.Order,
                CreatedDate = productImage.CreatedDate
            };
        }

        public static ProductVariantResponse MapModelToResponse(this ProductVariant productVariant)
        {
            var color = productVariant.Color != null ? productVariant.Color.MapModelToResponse() : null;
            var size = productVariant.Size != null ? productVariant.Size.MapModelToResponse() : null;

            return new ProductVariantResponse
            {
                Id = productVariant.Id,
                Price = productVariant.Price,
                StockQuantity = productVariant.StockQuantity,
                Color = color,
                Size = size,
                CreatedDate = productVariant.CreatedDate,
                UpdatedDate = productVariant.UpdatedDate
            };
        }
    }
}