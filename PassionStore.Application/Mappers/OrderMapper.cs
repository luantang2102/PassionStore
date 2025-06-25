using PassionStore.Application.DTOs.Orders;
using PassionStore.Core.Entities;

namespace PassionStore.Application.Mappers
{
    public static class OrderMapper
    {
        public static OrderResponse MapModelToResponse(this Order order)
        {
            
            return new OrderResponse
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                PaymentMethod = order.PaymentMethod,
                UserProfileId = order.UserProfileId,
                UserFullName = order.UserProfile?.FullName ?? string.Empty,
                ShippingAddress = order.ShippingAddress,
                OrderItems = order.OrderItems.Select(i => i.MapModelToResponse()).ToList(),
                CreatedDate = order.CreatedDate,
                UpdatedDate = order.UpdatedDate
            };
        }

        public static OrderItemResponse MapModelToResponse(this OrderItem orderItem)
        {
            return new OrderItemResponse
            {
                Id = orderItem.Id,
                Quantity = orderItem.Quantity,
                Price = orderItem.Price,
                ProductVariantId = orderItem.ProductVariantId,
                ProductName = orderItem.ProductVariant!.Product.Name,
                Color = orderItem.ProductVariant!.Color.MapModelToResponse(),
                Size = orderItem.ProductVariant!.Size.MapModelToResponse(),
                ProductId = orderItem.ProductVariant.ProductId,
                ProductDescription = orderItem.ProductVariant.Product?.Description ?? string.Empty,
            };
        }
    }
}
