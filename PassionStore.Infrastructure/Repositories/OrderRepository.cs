using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data;
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

        public async Task<Order?> GetByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(x => x.UserProfile)
                .Include(x => x.ShippingAddress)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Product)
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
                .Include(x => x.ShippingAddress)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Product)
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

        public async Task<Address?> GetAddressByIdAsync(Guid addressId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(x => x.Id == addressId);
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
    }
}