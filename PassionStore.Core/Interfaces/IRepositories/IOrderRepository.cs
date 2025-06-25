using PassionStore.Core.Entities;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order);
        Task DeleteAsync(Order order);
        Task<Order?> GetByIdAsync(Guid orderId);
        IQueryable<Order> GetByUserIdAsync(Guid userId);
        Task<UserProfile?> GetUserProfileByUserIdAsync(Guid userId);
        Task UpdateAsync(Order order);
    }
}
