using PassionStore.Core.Entities;
using System.Linq;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IRatingRepository
    {
        IQueryable<Rating> GetAllAsync();
        Task<Rating> GetByIdAsync(Guid ratingId);
        IQueryable<Rating> GetByProductIdAsync(Guid productId);
        Task<Rating> CreateAsync(Rating rating);
        Task UpdateAsync(Rating rating);
        Task DeleteAsync(Rating rating);
        Task<bool> HasRatedAsync(Guid userId, Guid productId);
        Task<Rating?> GetUserRatingForProductAsync(Guid userId, Guid productId);
    }
}