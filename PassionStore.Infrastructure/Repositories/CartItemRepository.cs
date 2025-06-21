using Microsoft.EntityFrameworkCore;
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

        public async Task<CartItem> CreateAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            return cartItem;
        }

        public async Task DeleteAsync(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await Task.CompletedTask;
        }

        public IQueryable<CartItem> GetAllAsync()
        {
            return _context.CartItems
                .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Color)
                .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Size)
                .Include(x => x.Cart);
        }

        public async Task<CartItem?> GetByIdAsync(Guid cartItemId)
        {
            return await _context.CartItems
                .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Color)
                .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Size)
                .Include(x => x.Cart)
                .FirstOrDefaultAsync(x => x.Id == cartItemId);
        }

        public async Task UpdateAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await Task.CompletedTask;
        }
    }
}