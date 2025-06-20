namespace PassionStore.Web.Services
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(OrderRequest request);
        Task<OrderResponse> GetOrderAsync(Guid orderId);
        Task UpdateOrderStatusAsync(Guid orderId, string status);
    }
}