using PassionStore.Core.Entities;
using PassionStore.Core.Models.Auth;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IRatingRepository
    {
        IQueryable<Rating> GetAllAsync();
        Task<Rating?> GetByIdAsync(Guid ratingId);
        IQueryable<Rating> GetByProductIdAsync(Guid productId);
        Task<Rating?> GetByUserAndProductAsync(Guid userId, Guid productId);
        Task<AppUser?> GetUserByIdAsync(Guid userId);
        Task<Rating> CreateAsync(Rating rating);
        Task UpdateAsync(Rating rating);
        Task DeleteAsync(Rating rating);
    }
}