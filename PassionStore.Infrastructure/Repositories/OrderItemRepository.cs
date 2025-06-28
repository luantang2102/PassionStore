using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Entities;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Infrastructure.Data;

namespace PassionStore.Infrastructure.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderItem> CreateAsync(OrderItem orderItem)
        {
            await _context.OrderItems.AddAsync(orderItem);
            return orderItem;
        }

        public async Task DeleteAsync(OrderItem orderItem)
        {
            _context.OrderItems.Remove(orderItem);
            await Task.CompletedTask;
        }

        public IQueryable<OrderItem> GetAllAsync()
        {
            return _context.OrderItems
                .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Color)
                .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Size)
                .Include(x => x.Order);
        }

        public async Task<OrderItem?> GetByIdAsync(Guid orderItemId)
        {
            return await _context.OrderItems
                .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Color)
                .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Size)
                .Include(x => x.Order)
                .FirstOrDefaultAsync(x => x.Id == orderItemId);
        }

        public async Task UpdateAsync(OrderItem orderItem)
        {
            _context.OrderItems.Update(orderItem);
            await Task.CompletedTask;
        }
    }
}