using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Entities;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PassionStore.Infrastructure.Repositories
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly AppDbContext _context;

        public ProductVariantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductVariant?> GetByIdAsync(Guid productVariantId)
        {
            return await _context.ProductVariants
                .Include(x => x.Product)
                .Include(x => x.Color)
                .Include(x => x.Size)
                .FirstOrDefaultAsync(x => x.Id == productVariantId);
        }

        public IQueryable<ProductVariant> GetAllAsync()
        {
            return _context.ProductVariants
                .Include(x => x.Product)
                .Include(x => x.Color)
                .Include(x => x.Size);
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _context.Products
                .FirstOrDefaultAsync(x => x.Id == productId);
        }

        public async Task<Color?> GetColorByIdAsync(Guid colorId)
        {
            return await _context.Colors
                .FirstOrDefaultAsync(x => x.Id == colorId);
        }

        public async Task<Size?> GetSizeByIdAsync(Guid sizeId)
        {
            return await _context.Sizes
                .FirstOrDefaultAsync(x => x.Id == sizeId);
        }

        public async Task<List<ProductVariant>> GetByProductIdAsync(Guid productId)
        {
            return await _context.ProductVariants
                .Include(x => x.Color)
                .Include(x => x.Size)
                .Where(x => x.ProductId == productId)
                .ToListAsync();
        }

        public async Task<ProductVariant?> GetWithImagesAsync(Guid productVariantId)
        {
            return await _context.ProductVariants
                .Include(x => x.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == productVariantId);
        }

        public async Task<ProductVariant> CreateAsync(ProductVariant productVariant)
        {
            await _context.ProductVariants.AddAsync(productVariant);
            return productVariant;
        }

        public async Task UpdateAsync(ProductVariant productVariant)
        {
            _context.ProductVariants.Update(productVariant);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(ProductVariant productVariant)
        {
            _context.ProductVariants.Remove(productVariant);
            await Task.CompletedTask;
        }

        public async Task<bool> HasProductVariantAsync(Guid productVariantId)
        {
            return await _context.CartItems.AnyAsync(x => x.ProductVariantId == productVariantId) ||
                   await _context.OrderItems.AnyAsync(x => x.ProductVariantId == productVariantId);
        }

        public async Task<bool> HasColorAsync(Guid colorId)
        {
            return await _context.ProductVariants.AnyAsync(x => x.ColorId == colorId);
        }

        public async Task<bool> HasSizeAsync(Guid sizeId)
        {
            return await _context.ProductVariants.AnyAsync(x => x.SizeId == sizeId);
        }

        public async Task<bool> ExistsAsync(Guid productId, Guid colorId, Guid sizeId)
        {
            return await _context.ProductVariants
                .AnyAsync(x => x.ProductId == productId && x.ColorId == colorId && x.SizeId == sizeId);
        }
    }
}