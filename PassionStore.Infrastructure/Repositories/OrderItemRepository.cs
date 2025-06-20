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

        public Task<OrderItem> CreateAsync(OrderItem orderItem)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(OrderItem orderItem)
        {
            throw new NotImplementedException();
        }

        public IQueryable<OrderItem> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<OrderItem?> GetByIdAsync(Guid orderItemId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(OrderItem orderItem)
        {
            throw new NotImplementedException();
        }
    }
}