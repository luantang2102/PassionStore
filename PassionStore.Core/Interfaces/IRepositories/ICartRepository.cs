using PassionStore.Core.Models;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface ICartRepository
    {
        IQueryable<Cart> GetAllAsync();
        Task<Cart?> GetByIdAsync(Guid cartId);
        Task<Cart?> GetByUserIdAsync(Guid userId);
        Task<Cart> CreateAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task<bool> HasProductAsync(Guid productId);
        Task<bool> HasProductVariantAsync(Guid productVariantId);
    }
}