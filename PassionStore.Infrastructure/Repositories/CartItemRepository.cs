using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data;

namespace PassionStore.Infrastructure.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _context;

        public CartItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<CartItem> CreateAsync(CartItem cartItem)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(CartItem cartItem)
        {
            throw new NotImplementedException();
        }

        public IQueryable<CartItem> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CartItem?> GetByIdAsync(Guid cartItemId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(CartItem cartItem)
        {
            throw new NotImplementedException();
        }
    }
}