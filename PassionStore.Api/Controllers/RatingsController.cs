using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.Ratings;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Paginations;
using System;
using System.Threading.Tasks;

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
        public async Task<ActionResult<PagedList<RatingResponse>>> GetRatings([FromQuery] RatingParams ratingParams)
        {
            var ratings = await _ratingService.GetRatingsAsync(ratingParams);
            Response.AddPaginationHeader(ratings.Metadata);
            return Ok(ratings);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<RatingResponse>> GetRatingById(Guid id)
        {
            var userId = User.GetUserId();
            var rating = await _ratingService.GetRatingByIdAsync(id, userId);
            return Ok(rating);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RatingResponse>> CreateRating([FromForm] RatingRequest ratingRequest)
        {
            var userId = User.GetUserId();
            var createdRating = await _ratingService.CreateRatingAsync(ratingRequest, userId);
            return CreatedAtAction(nameof(GetRatingById), new { id = createdRating.Id }, createdRating);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<RatingResponse>> UpdateRating(Guid id, [FromForm] RatingRequest ratingRequest)
        {
            var userId = User.GetUserId();
            var updatedRating = await _ratingService.UpdateRatingAsync(ratingRequest, id, userId);
            return Ok(updatedRating);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRating(Guid id)
        {
            var userId = User.GetUserId();
            await _ratingService.DeleteRatingAsync(id, userId);
            return NoContent();
        }

        [HttpPost("{id}/helpful")]
        [Authorize]
        public async Task<IActionResult> ToggleHelpful(Guid id)
        {
            var userId = User.GetUserId();
            await _ratingService.ToggleHelpfulAsync(id, userId);
            return NoContent();
        }

        [HttpGet("has-rated/{productId}")]
        [Authorize]
        public async Task<ActionResult<bool>> HasRatedProduct(Guid productId)
        {
            var userId = User.GetUserId();
            var hasRated = await _ratingService.HasRatedAsync(userId, productId);
            return Ok(hasRated);
        }
    }
}