using PassionStore.Core.Entities;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IRatingRepository
    {
        IQueryable<Rating> GetAllAsync();
        IQueryable<Rating> GetByProductIdAsync(Guid productId);
        Task<Rating?> GetByIdAsync(Guid ratingId);
        Task<Rating> CreateAsync(Rating rating);
        Task UpdateAsync(Rating rating);
        Task DeleteAsync(Rating rating);
        Task<bool> HasRatedAsync(Guid userId, Guid productId);
    }
}