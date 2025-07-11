using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Entities;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Infrastructure.Data;

namespace PassionStore.Infrastructure.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly AppDbContext _context;

        public RatingRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Rating> GetAllAsync()
        {
            return _context.Ratings
                .Include(x => x.Product)
                .Include(x => x.User);
        }

        public IQueryable<Rating> GetByProductIdAsync(Guid productId)
        {
            return _context.Ratings
                .Include(x => x.Product)
                .Include(x => x.User)
                .Where(x => x.ProductId == productId);
        }

        public async Task<Rating?> GetByIdAsync(Guid ratingId)
        {
            return await _context.Ratings
                .Include(x => x.Product)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == ratingId);
        }

        public async Task<Rating> CreateAsync(Rating rating)
        {
            await _context.Ratings.AddAsync(rating);
            return rating;
        }

        public async Task UpdateAsync(Rating rating)
        {
            _context.Ratings.Update(rating);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Rating rating)
        {
            _context.Ratings.Remove(rating);
            await Task.CompletedTask;
        }

        public async Task<bool> HasRatedAsync(Guid userId, Guid productId)
        {
            return await _context.Ratings
                .AnyAsync(x => x.UserId == userId && x.ProductId == productId);
        }

        public async Task<Rating?> GetUserRatingForProductAsync(Guid userId, Guid productId)
        {
            return await _context.Ratings
                .Include(x => x.Product)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
        }
    }
}