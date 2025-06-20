using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.Ratings;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;

namespace PassionStore.Api.Controllers
{
    public class RatingsController : BaseApiController
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRatings([FromQuery] RatingParams ratingParams)
        {
            var ratings = await _ratingService.GetRatingsAsync(ratingParams);
            Response.AddPaginationHeader(ratings.Metadata);
            return Ok(ratings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRatingById(Guid id)
        {
            var rating = await _ratingService.GetRatingByIdAsync(id);
            return Ok(rating);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetRatingsByProductId(Guid productId, [FromQuery] RatingParams ratingParams)
        {
            var ratings = await _ratingService.GetRatingsByProductIdAsync(ratingParams, productId);
            Response.AddPaginationHeader(ratings.Metadata);
            return Ok(ratings);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateRating([FromBody] RatingRequest ratingRequest)
        {
            var userId = User.GetUserId();
            var createdRating = await _ratingService.CreateRatingAsync(userId, ratingRequest);
            return CreatedAtAction(nameof(GetRatingById), new { id = createdRating.Id }, createdRating);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateRating(Guid id, [FromBody] RatingRequest ratingRequest)
        {
            var userId = User.GetUserId();
            var updatedRating = await _ratingService.UpdateRatingAsync(userId, id, ratingRequest);
            return Ok(updatedRating);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteRating(Guid id)
        {
            var userId = User.GetUserId();
            await _ratingService.DeleteRatingAsync(userId, id);
            return NoContent();
        }
    }
}