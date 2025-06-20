using PassionStore.Application.DTOs.Carts;

namespace PassionStore.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartResponse> GetCartByUserIdAsync(Guid userId);
        Task<CartResponse> AddCartItemAsync(Guid userId, CartItemRequest cartItemRequest);
        Task<CartResponse> UpdateCartItemAsync(Guid userId, Guid cartItemId, CartItemRequest cartItemRequest);
        Task DeleteCartItemAsync(Guid userId, Guid cartItemId);
    }
}