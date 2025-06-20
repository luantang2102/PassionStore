using PassionStore.Core.Models;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByIdAsync(Guid userId);
        Task UpdateAsync(UserProfile userProfile);
    }
}
