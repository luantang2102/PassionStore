using PassionStore.Application.DTOs.Users;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Mappers;
using PassionStore.Application.Paginations;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Infrastructure.Extensions;

namespace PassionStore.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse> GetUserByIdAsync(Guid userId)
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

            if (!await _userRepository.IsInRoleAsync(user, "User"))
            {
                var attribute = new Dictionary<string, object>
                    {
                        { "UserId", userId.ToString() }
                    };
                throw new AppException(ErrorCode.USER_NOT_FOUND, attribute);
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

        //public async Task<UserResponse> GetUserByIdAsync(Guid userId)
        //{
        //    
        //}
    }
}