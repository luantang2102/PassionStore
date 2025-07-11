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
                ShippingCost = (decimal)order.ShippingMethod,
                Status = order.Status.ToString(),
                OrderDate = order.OrderDate,
                PaymentMethod = order.PaymentMethod.ToString(),
                ShippingMethod = order.ShippingMethod.ToString(),
                PaymentLink = order.PaymentLink,
                PaymentTransactionId = order.PaymentTransactionId,
                UserProfile = order.UserProfile.MapModelToResponse(),
                UserFullName = order.UserProfile?.FullName ?? string.Empty,
                ShippingAddress = order.ShippingAddress,
                Note = order.Note,
                ReturnReason = order.ReturnReason,
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
                ProductImage = orderItem.ProductVariant.Product.ProductImages?.FirstOrDefault()?.ImageUrl ?? string.Empty,
                Color = orderItem.ProductVariant!.Color.MapModelToResponse(),
                Size = orderItem.ProductVariant!.Size.MapModelToResponse(),
                ProductId = orderItem.ProductVariant.ProductId,
                ProductDescription = orderItem.ProductVariant.Product?.Description ?? string.Empty,
            };
        }
    }
}