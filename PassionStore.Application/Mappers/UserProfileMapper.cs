using PassionStore.Application.DTOs.UserProfiles;
using PassionStore.Core.Models;


namespace PassionStore.Application.Mappers
{
    public static class UserProfileMapper
    {
        public static UserProfileResponse MapModelToResponse(this UserProfile userProfile)
        {
            return new UserProfileResponse
            {
                Id = userProfile.Id,
                PhoneNumber = userProfile.PhoneNumber,
                Address = userProfile.Address.MapModelToResponse(),
                CreatedDate = userProfile.CreatedDate,
                UpdatedDate = userProfile.UpdatedDate,
                //UserId = userProfile.UserId,
                //UserName = userProfile.User?.UserName,
                //Email = userProfile.User?.Email
            };
        }
    }
}
