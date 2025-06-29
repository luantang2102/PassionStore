using PassionStore.Application.DTOs.Ratings;
using PassionStore.Core.Entities;

namespace PassionStore.Application.Mappers
{
    public static class RatingMapper
    {
        public static RatingResponse MapModelToResponse(this Rating rating)
        {
            return new RatingResponse
            {
                Id = rating.Id,
                Value = rating.Value,
                Comment = rating.Comment,
                Helpful = rating.Helpful,
                CreatedDate = rating.CreatedDate,
                UpdatedDate = rating.UpdatedDate,
                ProductId = rating.ProductId,
                UserId = rating.UserId,
                UserName = rating.User.UserName,
                Email = rating.User.Email,
                ImageUrl = rating.User.ImageUrl,
            };
        }
    }
}
