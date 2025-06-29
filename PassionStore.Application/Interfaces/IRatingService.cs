using PassionStore.Application.DTOs.Ratings;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;

namespace PassionStore.Application.Interfaces
{
    public interface IRatingService
    {
        Task<PagedList<RatingResponse>> GetRatingsAsync(RatingParams ratingParams);
        Task<RatingResponse> GetRatingByIdAsync(Guid ratingId, Guid userId);
        Task<RatingResponse> CreateRatingAsync(RatingRequest ratingRequest, Guid userId);
        Task<RatingResponse> UpdateRatingAsync(RatingRequest ratingRequest, Guid ratingId, Guid userId);
        Task DeleteRatingAsync(Guid ratingId, Guid userId);
        Task ToggleHelpfulAsync(Guid ratingId, Guid userId);
        Task<bool> HasRatedAsync(Guid userId, Guid productId);
    }
}