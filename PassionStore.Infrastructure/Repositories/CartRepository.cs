// File: CartRepository.cs
using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data;

namespace PassionStore.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Cart> GetAllAsync()
        {
            return _context.Carts
                .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.Color)
                .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.Size)
                .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.ProductVariantImages);
        }

        public async Task<Cart?> GetByIdAsync(Guid cartId)
        {
            return await _context.Carts
                .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.Color)
                .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.Size)
                .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.ProductVariantImages)
                .FirstOrDefaultAsync(x => x.Id == cartId);
        }

        public async Task<Cart?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Carts
                .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.Color)
                .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.Size)
                .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x.ProductVariantImages)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Cart> CreateAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            return cart;
        }

        public async Task UpdateAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await Task.CompletedTask;
        }


        public async Task<bool> HasProductAsync(Guid productId)
        {
            return await _context.CartItems
                .AnyAsync(x => x.ProductVariant.ProductId == productId);
        }

        public async Task<bool> HasProductVariantAsync(Guid productVariantId)
        {
            return await _context.CartItems
                .AnyAsync(x => x.ProductVariantId == productVariantId);
        }
    }
}