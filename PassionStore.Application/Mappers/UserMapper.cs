using PassionStore.Application.DTOs.Identities;
using PassionStore.Application.DTOs.Users;
using PassionStore.Core.Models;
using PassionStore.Core.Models.Auth;

namespace PassionStore.Application.Mappers
{
    public static class UserMapper
    {
        public static AppUser MapToModel(this RegisterRequest registerRequest)
        {
            return new AppUser
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
                ImageUrl = registerRequest.ImageUrl,
                PublicId = registerRequest.PublicId,
                CreatedDate = DateTime.UtcNow,
                EmailConfirmed = false
            };
        }

        public static AppUser MapToModel(this GoogleLoginResponse googleUser)
        {
            return new AppUser
            {
                UserName = googleUser.Email.Split("@").ElementAt(0),
                Email = googleUser.Email,
                ImageUrl = googleUser.Picture,
                // PublicId = googleUser.Sub,
                CreatedDate = DateTime.UtcNow,
                EmailConfirmed = googleUser.EmailVerified
            };
        }

        public static UserResponse MapModelToResponse(this AppUser user, IList<string> roles = null!)
        {
            roles ??= [];
            if (user.Cart == null)
            {
                user.Cart = new Cart();
            }
            return new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                ImageUrl = user.ImageUrl,
                PublicId = user.PublicId,
                Email = user.Email,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdatedDate,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList(),
                UserProfiles = user.UserProfiles.Select(x => x.MapModelToResponse()).ToList(),
                Ratings = user.Ratings.Select(x => x.MapModelToResponse()).ToList(),
                CartItemsCount = user.Cart.CartItems?.Count ?? 0
            };
        }


    }
}
