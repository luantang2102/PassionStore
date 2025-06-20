using PassionStore.Application.DTOs.Carts;
using PassionStore.Application.DTOs.Products;
using PassionStore.Core.Models;

namespace PassionStore.Application.Mappers
{
    public static class CartMapper
    {
        public static CartResponse MapModelToResponse(this Cart cart)
        {
            return new CartResponse
            {
                Id = cart.Id,
                CartItems = cart.CartItems.Select(x => x.MapModelToResponse()).ToList(),
            };
        }

        public static CartItemResponse MapModelToResponse(this CartItem cartItem)
        {
            return new CartItemResponse
            {
                Id = cartItem.Id,
                ProductId = cartItem.ProductVariant.Product.Id,
                ProductVariantId = cartItem.ProductVariant.Id,
                ProductName = cartItem.ProductVariant.Product.Name,
                ProductDescription = cartItem.ProductVariant.Product.Description,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price,
                Images = cartItem.ProductVariant.ProductVariantImages.Select(x => x.MapModelToResponse()).ToList(),
                Color = cartItem.ProductVariant.Color.MapModelToResponse(),
                Size = cartItem.ProductVariant.Size.MapModelToResponse()
            };
        }
    }
}
