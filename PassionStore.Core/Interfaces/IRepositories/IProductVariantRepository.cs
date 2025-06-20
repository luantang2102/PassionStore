using PassionStore.Core.Entities;
using PassionStore.Core.Models;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IProductVariantRepository
    {
        Task<ProductVariant?> GetByIdAsync(Guid productVariantId);
        IQueryable<ProductVariant> GetAllAsync();
        Task<Product?> GetProductByIdAsync(Guid productId);
        Task<Color?> GetColorByIdAsync(Guid colorId);
        Task<Size?> GetSizeByIdAsync(Guid sizeId);
        Task<ProductVariant?> GetWithImagesAsync(Guid productVariantId);
        Task<ProductVariant> CreateAsync(ProductVariant productVariant);
        Task UpdateAsync(ProductVariant productVariant);
        Task DeleteAsync(ProductVariant productVariant);
        Task<bool> HasColorAsync(Guid colorId);
        Task<bool> HasSizeAsync(Guid sizeId);
    }
}