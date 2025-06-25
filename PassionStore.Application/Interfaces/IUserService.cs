using PassionStore.Application.DTOs.UserProfiles;
using PassionStore.Application.DTOs.Users;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;

namespace PassionStore.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileResponse> CreateUserProfileAsync(UserProfileRequest userProfileRequest, Guid userId);
        Task DeleteUserProfileAsync(Guid userId, Guid userProfileId);
        Task<UserResponse> GetUserByIdAsync(Guid userId, Guid currentUserId);
        Task<UserProfileResponse> GetUserProfileByIdAsync(Guid userId, Guid userProfileId);
        Task<PagedList<UserProfileResponse>> GetUserProfilesByUserIdAsync(UserProfileParams userProfileParams, Guid userId);
        Task<PagedList<UserResponse>> GetUsersAsync(UserParams userParams);
        Task<UserResponse> UpdateUserAsync(UserRequest userRequest, Guid userId, Guid currentUserId);
        Task<UserProfileResponse> UpdateUserProfileAsync(UserProfileRequest userProfileRequest, Guid userId, Guid userProfileId);
    }
}
