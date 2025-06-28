using PassionStore.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

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
    }
}