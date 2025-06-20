using PassionStore.Application.DTOs.Ratings;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;

namespace PassionStore.Application.Interfaces
{
    public interface IRatingService
    {
        Task<PagedList<RatingResponse>> GetRatingsAsync(RatingParams ratingParams);
        Task<RatingResponse> GetRatingByIdAsync(Guid ratingId);
        Task<PagedList<RatingResponse>> GetRatingsByProductIdAsync(RatingParams ratingParams, Guid productId);
        Task<RatingResponse> CreateRatingAsync(Guid userId, RatingRequest ratingRequest);
        Task<RatingResponse> UpdateRatingAsync(Guid userId, Guid ratingId, RatingRequest ratingRequest);
        Task DeleteRatingAsync(Guid userId, Guid ratingId);
    }
}