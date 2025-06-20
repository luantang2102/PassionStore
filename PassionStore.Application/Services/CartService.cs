using PassionStore.Application.DTOs.Carts;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Mappers;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;

namespace PassionStore.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(
            ICartRepository cartRepository,
            IProductVariantRepository productVariantRepository,
            IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _productVariantRepository = productVariantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CartResponse> GetCartByUserIdAsync(Guid userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                var attributes = new Dictionary<string, object> { { "userId", userId } };
                throw new AppException(ErrorCode.CART_NOT_FOUND, attributes);
            }
            return cart.MapModelToResponse();
        }

        public async Task<CartResponse> AddCartItemAsync(Guid userId, CartItemRequest cartItemRequest)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId, CartItems = [] };
                await _cartRepository.CreateAsync(cart);
            }

            var productVariant = await _productVariantRepository.GetByIdAsync(cartItemRequest.ProductVariantId);
            if (productVariant == null)
            {
                var attributes = new Dictionary<string, object> { { "productVariantId", cartItemRequest.ProductVariantId } };
                throw new AppException(ErrorCode.PRODUCT_VARIANT_NOT_FOUND, attributes);
            }

            if (productVariant.StockQuantity < cartItemRequest.Quantity)
            {
                var attributes = new Dictionary<string, object> { { "productVariantId", cartItemRequest.ProductVariantId } };
                throw new AppException(ErrorCode.INSUFFICIENT_STOCK, attributes);
            }

            var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductVariantId == cartItemRequest.ProductVariantId);
            if (existingItem != null)
            {
                existingItem.Quantity += cartItemRequest.Quantity;
                existingItem.Price = productVariant.Price;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductVariantId = cartItemRequest.ProductVariantId,
                    Quantity = cartItemRequest.Quantity,
                    Price = productVariant.Price,
                    CartId = cart.Id
                };
                cart.CartItems.Add(cartItem);
            }

            await _cartRepository.UpdateAsync(cart);
            await _unitOfWork.CommitAsync();
            return cart.MapModelToResponse();
        }

        public async Task<CartResponse> UpdateCartItemAsync(Guid userId, Guid cartItemId, CartItemRequest cartItemRequest)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                var attributes = new Dictionary<string, object> { { "userId", userId } };
                throw new AppException(ErrorCode.CART_NOT_FOUND, attributes);
            }

            var cartItem = cart.CartItems.FirstOrDefault(i => i.Id == cartItemId);
            if (cartItem == null)
            {
                var attributes = new Dictionary<string, object> { { "cartItemId", cartItemId } };
                throw new AppException(ErrorCode.CART_ITEM_NOT_FOUND, attributes);
            }

            var productVariant = await _productVariantRepository.GetByIdAsync(cartItemRequest.ProductVariantId);
            if (productVariant == null)
            {
                var attributes = new Dictionary<string, object> { { "productVariantId", cartItemRequest.ProductVariantId } };
                throw new AppException(ErrorCode.PRODUCT_VARIANT_NOT_FOUND, attributes);
            }

            if (productVariant.StockQuantity < cartItemRequest.Quantity)
            {
                var attributes = new Dictionary<string, object> { { "productVariantId", cartItemRequest.ProductVariantId } };
                throw new AppException(ErrorCode.INSUFFICIENT_STOCK, attributes);
            }

            cartItem.ProductVariantId = cartItemRequest.ProductVariantId;
            cartItem.Quantity = cartItemRequest.Quantity;
            cartItem.Price = productVariant.Price;

            await _cartRepository.UpdateAsync(cart);
            await _unitOfWork.CommitAsync();
            return cart.MapModelToResponse();
        }

        public async Task DeleteCartItemAsync(Guid userId, Guid cartItemId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                var attributes = new Dictionary<string, object> { { "userId", userId } };
                throw new AppException(ErrorCode.CART_NOT_FOUND, attributes);
            }

            var cartItem = cart.CartItems.FirstOrDefault(i => i.Id == cartItemId);
            if (cartItem == null)
            {
                var attributes = new Dictionary<string, object> { { "cartItemId", cartItemId } };
                throw new AppException(ErrorCode.CART_ITEM_NOT_FOUND, attributes);
            }

            cart.CartItems.Remove(cartItem);
            await _cartRepository.UpdateAsync(cart);
            await _unitOfWork.CommitAsync();
        }
    }
}