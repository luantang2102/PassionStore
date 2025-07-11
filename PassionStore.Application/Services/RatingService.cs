using PassionStore.Application.DTOs.Ratings;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Mappers;
using PassionStore.Application.Paginations;
using PassionStore.Core.Entities;
using PassionStore.Core.Enums;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Infrastructure.Extensions;

namespace PassionStore.Application.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHelpfulVoteRepository _helpfulVoteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RatingService(
            IRatingRepository ratingRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IHelpfulVoteRepository helpfulVoteRepository,
            IUnitOfWork unitOfWork)
        {
            _ratingRepository = ratingRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _helpfulVoteRepository = helpfulVoteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<RatingResponse>> GetRatingsAsync(RatingParams ratingParams)
        {
            var query = _ratingRepository.GetAllAsync()
                .Filter(ratingParams.Values, ratingParams.HasComment)
                .Search(ratingParams.SearchTerm)
                .Sort(ratingParams.OrderBy);

            var projectedQuery = query.Select(r => r.MapModelToResponse());

            return await PaginationService.ToPagedList(
                projectedQuery,
                ratingParams.PageNumber,
                ratingParams.PageSize
            );
        }

        public async Task<PagedList<RatingResponse>> GetRatingsByProductIdAsync(Guid productId, RatingParams ratingParams)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                var attributes = new Dictionary<string, object> { { "productId", productId } };
                throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
            }

            var query = _ratingRepository.GetByProductIdAsync(productId)
                .Filter(ratingParams.Values, ratingParams.HasComment)
                .Search(ratingParams.SearchTerm)
                .Sort(ratingParams.OrderBy);

            var projectedQuery = query.Select(r => r.MapModelToResponse());

            return await PaginationService.ToPagedList(
                projectedQuery,
                ratingParams.PageNumber,
                ratingParams.PageSize
            );
        }

        public async Task<RatingResponse> GetRatingByIdAsync(Guid ratingId, Guid userId)
        {
            var rating = await _ratingRepository.GetByIdAsync(ratingId);
            if (rating == null)
            {
                var attributes = new Dictionary<string, object> { { "ratingId", ratingId } };
                throw new AppException(ErrorCode.RATING_NOT_FOUND, attributes);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }

            if (!await _userRepository.IsInRoleAsync(user, "Admin") && rating.UserId != userId)
            {
                throw new AppException(ErrorCode.ACCESS_DENIED);
            }

            return rating.MapModelToResponse();
        }

        public async Task<RatingResponse> CreateRatingAsync(RatingRequest ratingRequest, Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }

            var product = await _productRepository.GetByIdAsync(ratingRequest.ProductId);
            if (product == null)
            {
                var attributes = new Dictionary<string, object> { { "productId", ratingRequest.ProductId } };
                throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
            }

            var hasPurchased = await _orderRepository.HasUserPurchasedProductAsync(userId, ratingRequest.ProductId);
            if (!hasPurchased)
            {
                var attributes = new Dictionary<string, object> { { "productId", ratingRequest.ProductId }, { "userId", userId } };
                throw new AppException(ErrorCode.USER_NOT_PURCHASED_PRODUCT, attributes);
            }

            var hasRated = await _ratingRepository.HasRatedAsync(userId, ratingRequest.ProductId);
            if (hasRated)
            {
                var attributes = new Dictionary<string, object> { { "productId", ratingRequest.ProductId }, { "userId", userId } };
                throw new AppException(ErrorCode.USER_ALREADY_RATED, attributes);
            }

            // Check if the order is completed and within 3 months
            var order = await _orderRepository.GetCompletedOrderForProductAsync(userId, ratingRequest.ProductId);
            if (order == null || order.Status != OrderStatus.Completed)
            {
                var attributes = new Dictionary<string, object> { { "productId", ratingRequest.ProductId }, { "userId", userId } };
                throw new AppException(ErrorCode.ORDER_NOT_COMPLETED, attributes);
            }

            var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);
            if (order.UpdatedDate < threeMonthsAgo)
            {
                var attributes = new Dictionary<string, object> { { "productId", ratingRequest.ProductId }, { "userId", userId } };
                throw new AppException(ErrorCode.RATING_WINDOW_EXPIRED, attributes);
            }

            var rating = new Rating
            {
                Value = ratingRequest.Value,
                Comment = ratingRequest.Comment ?? string.Empty,
                ProductId = ratingRequest.ProductId,
                UserId = userId,
                Helpful = 0,
                CreatedDate = DateTime.UtcNow
            };

            var createdRating = await _ratingRepository.CreateAsync(rating);
            await _unitOfWork.CommitAsync();

            return createdRating.MapModelToResponse();
        }

        public async Task<RatingResponse> UpdateRatingAsync(RatingRequest ratingRequest, Guid ratingId, Guid userId)
        {
            var rating = await _ratingRepository.GetByIdAsync(ratingId);
            if (rating == null)
            {
                var attributes = new Dictionary<string, object> { { "ratingId", ratingId } };
                throw new AppException(ErrorCode.RATING_NOT_FOUND, attributes);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }

            if (!await _userRepository.IsInRoleAsync(user, "Admin") && rating.UserId != userId)
            {
                throw new AppException(ErrorCode.ACCESS_DENIED);
            }

            var product = await _productRepository.GetByIdAsync(ratingRequest.ProductId);
            if (product == null)
            {
                var attributes = new Dictionary<string, object> { { "productId", ratingRequest.ProductId } };
                throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
            }

            rating.Value = ratingRequest.Value;
            rating.Comment = ratingRequest.Comment ?? string.Empty;
            rating.ProductId = ratingRequest.ProductId;
            rating.UpdatedDate = DateTime.UtcNow;

            await _ratingRepository.UpdateAsync(rating);
            await _unitOfWork.CommitAsync();

            return rating.MapModelToResponse();
        }

        public async Task DeleteRatingAsync(Guid ratingId, Guid userId)
        {
            var rating = await _ratingRepository.GetByIdAsync(ratingId);
            if (rating == null)
            {
                var attributes = new Dictionary<string, object> { { "ratingId", ratingId } };
                throw new AppException(ErrorCode.RATING_NOT_FOUND, attributes);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }

            if (!await _userRepository.IsInRoleAsync(user, "Admin") && rating.UserId != userId)
            {
                throw new AppException(ErrorCode.ACCESS_DENIED);
            }

            await _ratingRepository.DeleteAsync(rating);
            await _unitOfWork.CommitAsync();
        }

        public async Task ToggleHelpfulAsync(Guid ratingId, Guid userId)
        {
            var rating = await _ratingRepository.GetByIdAsync(ratingId);
            if (rating == null)
            {
                var attributes = new Dictionary<string, object> { { "ratingId", ratingId } };
                throw new AppException(ErrorCode.RATING_NOT_FOUND, attributes);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }

            var existingVote = await _helpfulVoteRepository.GetByUserAndRatingAsync(userId, ratingId);

            if (existingVote == null)
            {
                rating.Helpful++;
                var helpfulVote = new HelpfulVote
                {
                    RatingId = ratingId,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };
                await _helpfulVoteRepository.CreateAsync(helpfulVote);
            }
            else
            {
                rating.Helpful = Math.Max(0, rating.Helpful - 1);
                await _helpfulVoteRepository.DeleteAsync(existingVote);
            }

            await _ratingRepository.UpdateAsync(rating);
            await _unitOfWork.CommitAsync();
        }

        public async Task<RatingResponse?> GetUserRatingForProductAsync(Guid userId, Guid productId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                var attributes = new Dictionary<string, object> { { "productId", productId } };
                throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
            }

            var rating = await _ratingRepository.GetUserRatingForProductAsync(userId, productId);
            return rating?.MapModelToResponse();
        }
    }
}