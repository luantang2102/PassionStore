using PassionStore.Application.DTOs.Orders;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;

namespace PassionStore.Application.Interfaces
{
    public interface IOrderService
    {
        Task<PagedList<OrderResponse>> GetOrdersAsync(OrderParams orderParams);
        Task<OrderResponse> GetOrderByIdAsync(Guid userId, Guid orderId);
        Task<PagedList<OrderResponse>> GetSelfOrdersAsync(Guid userId, OrderParams orderParams);
        Task<OrderResponse> CreateOrderAsync(Guid userId, OrderRequest orderRequest);
        Task<OrderResponse> UpdateOrderStatusAsync(Guid orderId, OrderStatusRequest orderStatusRequest);
        Task CancelOrderAsync(Guid userId, Guid orderId, string? cancellationReason, bool callBack = false);
        Task<OrderResponse?> HandlePaymentCallbackAsync(string code, string id, bool cancel, string status, long orderCode);
    }
}