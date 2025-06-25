using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models.Auth;
using PassionStore.Infrastructure.Data;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public UserRepository(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> AddToRoleAsync(AppUser user, string role)
    {
        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<bool> ChangePasswordAsync(AppUser user, string newPassword)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> CheckPasswordAsync(AppUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<bool> CreateAsync(AppUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    public async Task<AppUser?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public IQueryable<AppUser> GetAllAsync()
    {
        return _context.Users
            .Include(x => x.UserProfiles)
            .Include(x => x.Cart)
            .Include(x => x.Ratings);
    }

    public async Task<AppUser?> GetByIdAsync(Guid userId)
    {
        return await _context.Users
            .Include(x => x.UserProfiles)
            .Include(x => x.Cart)
            .ThenInclude(x => x.CartItems)
            .Include(x => x.Ratings)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task<IList<string>> GetRolesAsync(AppUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> UpdateAsync(AppUser user)
    {
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<List<AppUser>> GetUsersInRoleAsync(string role)
    {
        return (await _userManager.GetUsersInRoleAsync(role)).ToList();
    }

    public async Task<bool> IsInRoleAsync(AppUser user, string role)
    {
        return await _userManager.IsInRoleAsync(user, role);
    }
}