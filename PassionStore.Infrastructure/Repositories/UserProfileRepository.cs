using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Entities;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Infrastructure.Data;

namespace PassionStore.Infrastructure.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly AppDbContext _context;

        public UserProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile> CreateAsync(UserProfile userProfile)
        {
            await _context.UserProfiles.AddAsync(userProfile);
            return userProfile;
        }

        public Task DeleteAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Remove(userProfile);
            return Task.CompletedTask;
        }

        public async Task<UserProfile?> GetByIdAsync(Guid userId)
        {
            return await _context.UserProfiles
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public IQueryable<UserProfile> GetByUserIdAsync(Guid userId)
        {
            return _context.UserProfiles
                .Where(x => x.UserId == userId);
        }

        public async Task UpdateAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();
        }
    }
}