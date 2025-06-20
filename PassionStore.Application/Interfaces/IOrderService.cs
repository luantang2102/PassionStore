using PassionStore.Application.DTOs.Orders;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;

namespace PassionStore.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> GetOrderByIdAsync(Guid userId, Guid orderId);
        Task<PagedList<OrderResponse>> GetOrdersByUserIdAsync(Guid userId, OrderParams orderParams);
        Task<OrderResponse> CreateOrderAsync(Guid userId, OrderRequest orderRequest);
        Task<OrderResponse> UpdateOrderStatusAsync(Guid orderId, OrderStatusRequest orderStatusRequest);
        Task CancelOrderAsync(Guid userId, Guid orderId);
    }
}