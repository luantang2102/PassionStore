using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
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

        public async Task<UserProfile?> GetByIdAsync(Guid userId)
        {
            return await _context.UserProfiles
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task UpdateAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();
        }
    }
}