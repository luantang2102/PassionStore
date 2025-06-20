using PassionStore.Core.Models.Auth;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        IQueryable<AppUser> GetAllAsync();
        Task<AppUser?> GetByIdAsync(Guid userId);
        Task<IList<string>> GetRolesAsync(AppUser user);
        Task<AppUser?> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(AppUser user, string password);
        Task<bool> ChangePasswordAsync(AppUser user, string newPassword);
        Task<bool> UpdateAsync(AppUser user);
        Task<bool> CreateAsync(AppUser user, string password);
        Task<bool> AddToRoleAsync(AppUser user, string role);
        Task<List<AppUser>> GetUsersInRoleAsync(string role);
        Task<bool> IsInRoleAsync(AppUser user, string role);
    }
}