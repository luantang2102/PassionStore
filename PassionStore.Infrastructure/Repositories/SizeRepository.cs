using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PassionStore.Infrastructure.Repositories
{
    public class SizeRepository : ISizeRepository
    {
        private readonly AppDbContext _context;

        public SizeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Size?> GetByIdAsync(Guid sizeId)
        {
            return await _context.Sizes
                .FirstOrDefaultAsync(x => x.Id == sizeId);
        }

        public IQueryable<Size> GetAllAsync()
        {
            return _context.Sizes;
        }

        public async Task<Size> CreateAsync(Size size)
        {
            await _context.Sizes.AddAsync(size);
            return size;
        }

        public async Task UpdateAsync(Size size)
        {
            _context.Sizes.Update(size);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Size size)
        {
            _context.Sizes.Remove(size);
            await Task.CompletedTask;
        }
    }
}