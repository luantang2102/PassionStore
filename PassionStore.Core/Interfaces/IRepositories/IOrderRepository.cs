using PassionStore.Core.Entities;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IOrderRepository
    {
        IQueryable<Order> GetAllAsync();
        Task<Order?> GetByIdAsync(Guid orderId);
        IQueryable<Order> GetByUserIdAsync(Guid userId);
        Task<UserProfile?> GetUserProfileByUserIdAsync(Guid userId);
        Task<Order> CreateAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order);
        Task<Order?> GetByPaymentTransactionIdAsync(string paymentTransactionId);
        Task<bool> HasUserPurchasedProductAsync(Guid userId, Guid productId);
        Task<Order?> GetCompletedOrderForProductAsync(Guid userId, Guid productId);
    }
}