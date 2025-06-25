using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data;

namespace PassionStore.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;

        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Brand?> GetByIdAsync(Guid brandId)
        {
            return await _context.Brands
                .FirstOrDefaultAsync(x => x.Id == brandId);
        }

        public IQueryable<Brand> GetAllAsync()
        {
            return _context.Brands;
        }

        public async Task<Brand> CreateAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
            return brand;
        }

        public async Task UpdateAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Brand brand)
        {
            _context.Brands.Remove(brand);
            await Task.CompletedTask;
        }
    }
}