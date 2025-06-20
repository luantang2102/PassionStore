using PassionStore.Core.Entities;
using PassionStore.Core.Models;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid productId);
        IQueryable<Product> GetAllAsync();
        IQueryable<Product> GetByCategoryIdAsync(Guid categoryId);
        Task<Product> CreateAsync(Product product);
        Task<Product?> GetWithImagesAsync(Guid productId);
        Task<Product?> GetWithCategoriesAsync(Guid productId);
        Task<List<Category>> GetCategoriesByIdsAsync(List<Guid> categoryIds);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<bool> HasBrandAsync(Guid brandId);
    }
}