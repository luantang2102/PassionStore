using PassionStore.Core.Entities;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByIdAsync(Guid userId);
        IQueryable<UserProfile> GetByUserIdAsync(Guid userId);
        Task<UserProfile> CreateAsync(UserProfile userProfile);
        Task UpdateAsync(UserProfile userProfile);
        Task DeleteAsync(UserProfile userProfile);
    }
}
