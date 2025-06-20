using PassionStore.Core.Models;

namespace PassionStore.Infrastructure.Extensions
{
    public static class ProductVariantExtensions
    {
        public static IQueryable<ProductVariant> Sort(this IQueryable<ProductVariant> query, string? orderBy)
        {
            query = orderBy switch
            {
                "priceAsc" => query.OrderBy(x => x.Price),
                "priceDesc" => query.OrderByDescending(x => x.Price),
                "stockAsc" => query.OrderBy(x => x.StockQuantity),
                "stockDesc" => query.OrderByDescending(x => x.StockQuantity),
                _ => query.OrderBy(x => x.CreatedDate),
            };
            return query;
        }

        public static IQueryable<ProductVariant> Filter(this IQueryable<ProductVariant> query, Guid? productId, Guid? colorId, Guid? sizeId, string? minPrice, string? maxPrice)
        {
            if (productId.HasValue)
            {
                query = query.Where(x => x.ProductId == productId.Value);
            }

            if (colorId.HasValue)
            {
                query = query.Where(x => x.ColorId == colorId.Value);
            }

            if (sizeId.HasValue)
            {
                query = query.Where(x => x.SizeId == sizeId.Value);
            }

            if (!string.IsNullOrEmpty(minPrice) && decimal.TryParse(minPrice, out var parsedMinPrice))
            {
                query = query.Where(x => x.Price >= parsedMinPrice);
            }

            if (!string.IsNullOrEmpty(maxPrice) && decimal.TryParse(maxPrice, out var parsedMaxPrice))
            {
                query = query.Where(x => x.Price <= parsedMaxPrice);
            }

            return query;
        }
    }
}