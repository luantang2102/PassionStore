using Microsoft.EntityFrameworkCore;
using PassionStore.Application.DTOs.UserProfiles;
using PassionStore.Application.DTOs.Users;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Mappers;
using PassionStore.Application.Paginations;
using PassionStore.Core.Entities;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Extensions;
using PassionStore.Infrastructure.Externals;

namespace PassionStore.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly CloudinaryService cloudinaryService;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            IUserRepository userRepository,
            IUserProfileRepository userProfileRepository,
            CloudinaryService cloudinaryService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            this.cloudinaryService = cloudinaryService;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserResponse> GetUserByIdAsync(Guid userId, Guid currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                var attribute = new Dictionary<string, object>
                    {
                        { "UserId", userId.ToString() }
                    };
                throw new AppException(ErrorCode.USER_NOT_FOUND, attribute);
            }

            if (!await _userRepository.IsInRoleAsync(user, "Admin"))
            {
                if (user.Id != currentUserId)
                {
                    var attribute = new Dictionary<string, object>
                        {
                            { "UserId", userId.ToString() },
                            { "CurrentUserId", currentUserId.ToString() }
                        };
                    throw new AppException(ErrorCode.ACCESS_DENIED, attribute);
                }
            }

            var roles = await _userRepository.GetRolesAsync(user);

            return user.MapModelToResponse(roles);
        }

        public async Task<PagedList<UserResponse>> GetUsersAsync(UserParams userParams)
        {
            var usersInRole = await _userRepository.GetUsersInRoleAsync("User");
            var userIdsInRole = usersInRole.Select(u => u.Id).ToHashSet();

            var query = _userRepository.GetAllAsync()
                .Where(u => userIdsInRole.Contains(u.Id))
                .Search(userParams.SearchTerm)
                .Sort(userParams.OrderBy);

            var pagedList = await PaginationService.ToPagedList(
                query,
                userParams.PageNumber,
                userParams.PageSize
            );

            var usersWithRoles = new List<UserResponse>();
            foreach (var user in pagedList)
            {
                var roles = await _userRepository.GetRolesAsync(user);
                usersWithRoles.Add(user.MapModelToResponse(roles));
            }

            return new PagedList<UserResponse>(
                usersWithRoles,
                pagedList.Metadata.TotalCount,
                userParams.PageNumber,
                userParams.PageSize
            );
        }

        public async Task<UserResponse> UpdateUserAsync(UserRequest userRequest, Guid userId, Guid currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                var attribute = new Dictionary<string, object>
                    {
                        { "UserId", userId.ToString() }
                    };
                throw new AppException(ErrorCode.USER_NOT_FOUND, attribute);
            }
            if (!await _userRepository.IsInRoleAsync(user, "Admin") && user.Id != currentUserId)
            {
                var attribute = new Dictionary<string, object>
                    {
                        { "UserId", userId.ToString() },
                        { "CurrentUserId", currentUserId.ToString() }
                    };
                throw new AppException(ErrorCode.ACCESS_DENIED, attribute);
            }

            if (userRequest.Image != null)
            {
                if (!string.IsNullOrEmpty(user.PublicId))
                {
                    await cloudinaryService.DeleteImageAsync(user.PublicId);
                }
                var uploadResult = await cloudinaryService.AddImageAsync(userRequest.Image);
                user.ImageUrl = uploadResult.Url.ToString();
                user.PublicId = uploadResult.PublicId;
            }


            user.Gender = userRequest.Gender ?? user.Gender;
            user.DateOfBirth = userRequest.DateOfBirth ?? user.DateOfBirth; 
            user.UpdatedDate = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            await _unitOfWork.CommitAsync();

            return user.MapModelToResponse();
        }

        public async Task<PagedList<UserProfileResponse>> GetUserProfilesByUserIdAsync(UserProfileParams userProfileParams, Guid userId)
        {
            var query = _userProfileRepository.GetByUserIdAsync(userId)
                .Search(userProfileParams.SearchTerm)
                .Sort(userProfileParams.OrderBy);

            var projectedQuery = query.Select(x => x.MapModelToResponse());

            return await PaginationService.ToPagedList(
                projectedQuery,
                userProfileParams.PageNumber,
                userProfileParams.PageSize
            );
        }

        public async Task<UserProfileResponse> GetUserProfileByIdAsync(Guid userId, Guid userProfileId)
        {
            var userProfile = await _userProfileRepository.GetByIdAsync(userProfileId);
            if (userProfile == null || userProfile.Id != userId)
            {
                var attribute = new Dictionary<string, object>
                    {
                        { "UserProfileId", userProfileId.ToString() },
                        { "UserId", userId.ToString() }
                    };
                throw new AppException(ErrorCode.USER_PROFILE_NOT_FOUND, attribute);
            }
            return userProfile.MapModelToResponse();
        }

        public async Task<UserProfileResponse> CreateUserProfileAsync(UserProfileRequest userProfileRequest, Guid userId)
        {
            var userProfile = new UserProfile
            {
                FullName = userProfileRequest.FullName,
                PhoneNumber = userProfileRequest.PhoneNumber,
                Province = userProfileRequest.Province,
                District = userProfileRequest.District,
                Ward = userProfileRequest.Ward,
                SpecificAddress = userProfileRequest.SpecificAddress,
                IsDefault = userProfileRequest.IsDefault,
                UserId = userId
            };

            if (userProfileRequest.IsDefault)
            {
                var existingProfiles = await _userProfileRepository.GetByUserIdAsync(userId).ToListAsync();
                foreach (var profile in existingProfiles)
                {
                    if (profile.IsDefault)
                    {
                        profile.IsDefault = false;
                        await _userProfileRepository.UpdateAsync(profile);
                    }
                }
            }

            var createdUserProfile = await _userProfileRepository.CreateAsync(userProfile);

            await _unitOfWork.CommitAsync();

            return createdUserProfile.MapModelToResponse();
        }

        public async Task<UserProfileResponse> UpdateUserProfileAsync(UserProfileRequest userProfileRequest, Guid userId, Guid userProfileId)
        {
            var userProfile = await _userProfileRepository.GetByIdAsync(userProfileId);
            if (userProfile == null || userProfile.UserId != userId)
            {
                var attribute = new Dictionary<string, object>
                    {
                        { "UserProfileId", userProfileId.ToString() },
                        { "UserId", userId.ToString() }
                    };
                throw new AppException(ErrorCode.USER_PROFILE_NOT_FOUND, attribute);
            }
            userProfile.FullName = userProfileRequest.FullName;
            userProfile.PhoneNumber = userProfileRequest.PhoneNumber;
            userProfile.Province = userProfileRequest.Province;
            userProfile.District = userProfileRequest.District;
            userProfile.Ward = userProfileRequest.Ward;
            userProfile.SpecificAddress = userProfileRequest.SpecificAddress;
            userProfile.IsDefault = userProfileRequest.IsDefault;
            userProfile.UpdatedDate = DateTime.UtcNow;

            if (userProfileRequest.IsDefault)
            {
                var existingProfiles = await _userProfileRepository.GetByUserIdAsync(userId).ToListAsync();
                foreach (var profile in existingProfiles)
                {
                    if (profile.IsDefault && profile.Id != userProfileId)
                    {
                        profile.IsDefault = false;
                        await _userProfileRepository.UpdateAsync(profile);
                    }
                }
            }

            await _userProfileRepository.UpdateAsync(userProfile);

            await _unitOfWork.CommitAsync();

            return userProfile.MapModelToResponse();
        }

        public async Task DeleteUserProfileAsync(Guid userId, Guid userProfileId)
        {
            var userProfile = await _userProfileRepository.GetByIdAsync(userProfileId);
            if (userProfile == null || userProfile.UserId != userId)
            {
                var attribute = new Dictionary<string, object>
                    {
                        { "UserProfileId", userProfileId.ToString() },
                        { "UserId", userId.ToString() }
                    };
                throw new AppException(ErrorCode.USER_PROFILE_NOT_FOUND, attribute);
            }

            await _userProfileRepository.DeleteAsync(userProfile);

            await _unitOfWork.CommitAsync();
        }
    }
}