using PassionStore.Core.Models;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IColorRepository
    {
        Task<Color?> GetByIdAsync(Guid colorId);
        IQueryable<Color> GetAllAsync();
        Task<Color> CreateAsync(Color color);
        Task UpdateAsync(Color color);
        Task DeleteAsync(Color color);
    }
}