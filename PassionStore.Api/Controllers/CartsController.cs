using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.Carts;
using PassionStore.Application.Interfaces;

namespace PassionStore.Api.Controllers
{
    [Authorize]
    public class CartsController : BaseApiController
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.GetUserId();
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            return Ok(cart);
        }

        [HttpPost("me")]
        public async Task<IActionResult> AddCartItem([FromForm] CartItemRequest cartItemRequest)
        {
            var userId = User.GetUserId();
            var cart = await _cartService.AddCartItemAsync(userId, cartItemRequest);
            return Ok(cart);
        }

        [HttpPut("item/{id}")]
        public async Task<IActionResult> UpdateCartItem(Guid id, [FromForm] CartItemRequest cartItemRequest)
        {
            var userId = User.GetUserId();
            var cart = await _cartService.UpdateCartItemAsync(userId, id, cartItemRequest);
            return Ok(cart);
        }

        [HttpDelete("item/{id}")]
        public async Task<IActionResult> DeleteCartItem(Guid id)
        {
            var userId = User.GetUserId();
            await _cartService.DeleteCartItemAsync(userId, id);
            return NoContent();
        }
    }
}