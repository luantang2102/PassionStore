using PassionStore.Application.DTOs.UserProfiles;
using PassionStore.Core.Entities;

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
                FullName = userProfile.FullName,
                Province = userProfile.Province,
                District = userProfile.District,
                Ward = userProfile.Ward,
                SpecificAddress = userProfile.SpecificAddress,
                IsDefault = userProfile.IsDefault,
                CreatedDate = userProfile.CreatedDate,
                UpdatedDate = userProfile.UpdatedDate,
            };
        }
    }
}
