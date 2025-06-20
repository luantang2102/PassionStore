using PassionStore.Application.DTOs.Users;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;

namespace PassionStore.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> GetUserByIdAsync(Guid userId);
        Task<PagedList<UserResponse>> GetUsersAsync(UserParams userParams);
    }
}
