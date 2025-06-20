using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PassionStore.Infrastructure.Repositories
{
    public class ColorRepository : IColorRepository
    {
        private readonly AppDbContext _context;

        public ColorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Color?> GetByIdAsync(Guid colorId)
        {
            return await _context.Colors
                .FirstOrDefaultAsync(x => x.Id == colorId);
        }

        public IQueryable<Color> GetAllAsync()
        {
            return _context.Colors;
        }

        public async Task<Color> CreateAsync(Color color)
        {
            await _context.Colors.AddAsync(color);
            return color;
        }

        public async Task UpdateAsync(Color color)
        {
            _context.Colors.Update(color);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Color color)
        {
            _context.Colors.Remove(color);
            await Task.CompletedTask;
        }
    }
}