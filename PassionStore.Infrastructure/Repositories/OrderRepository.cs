using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Entities;
using PassionStore.Core.Enums;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PassionStore.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Order> GetAllAsync()
        {
            return _context.Orders
                .Include(x => x.UserProfile)
                    .ThenInclude(x => x.User)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Product)
                            .ThenInclude(x => x.ProductImages)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Color)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Size);
        }

        public async Task<Order?> GetByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(x => x.UserProfile)
                    .ThenInclude(x => x.User)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Product)
                            .ThenInclude(x => x.ProductImages)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Color)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Size)
                .FirstOrDefaultAsync(x => x.Id == orderId);
        }

        public IQueryable<Order> GetByUserIdAsync(Guid userId)
        {
            return _context.Orders
                .Include(x => x.UserProfile)
                    .ThenInclude(x => x.User)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Product)
                            .ThenInclude(x => x.ProductImages)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Color)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Size)
                .Where(x => x.UserProfile.UserId == userId);
        }

        public async Task<UserProfile?> GetUserProfileByUserIdAsync(Guid userId)
        {
            return await _context.UserProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Order> CreateAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            return order;
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Order order)
        {
            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);
            await Task.CompletedTask;
        }

        public async Task<Order?> GetByPaymentTransactionIdAsync(string paymentTransactionId)
        {
            return await _context.Orders
                .Include(x => x.UserProfile)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Product)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Color)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Size)
                .FirstOrDefaultAsync(x => x.PaymentTransactionId == paymentTransactionId);
        }

        public async Task<bool> HasUserPurchasedProductAsync(Guid userId, Guid productId)
        {
            return await _context.Orders
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                .Where(x => x.UserProfile.UserId == userId && x.Status == OrderStatus.Completed)
                .AnyAsync(x => x.OrderItems.Any(oi => oi.ProductVariant.ProductId == productId));
        }

        public async Task<Order?> GetCompletedOrderForProductAsync(Guid userId, Guid productId)
        {
            return await _context.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
                .Include(x => x.UserProfile)
                .Where(x => x.UserProfile.UserId == userId &&
                            x.OrderItems.Any(oi => oi.ProductVariant.ProductId == productId) &&
                            x.Status == OrderStatus.Completed)
                .OrderByDescending(x => x.UpdatedDate)
                .FirstOrDefaultAsync();
        }
    }
}