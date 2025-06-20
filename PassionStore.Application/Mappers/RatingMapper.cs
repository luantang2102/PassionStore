using PassionStore.Application.DTOs.Ratings;
using PassionStore.Core.Entities;

namespace PassionStore.Application.Mappers
{
    public static class RatingMapper
    {
        public static RatingResponse MapModelToReponse(this Rating rating)
        {
            return new RatingResponse
            {
                Id = rating.Id,
                Value = rating.Value,
                Comment = rating.Comment,
                CreatedDate = rating.CreatedDate,
                ProductId = rating.ProductId,
                UserId = rating.UserId,
                UserName = rating.User.UserName,
                ImageUrl = rating.User.ImageUrl,
                PublicId = rating.User.PublicId
            };
        }
    }
}
