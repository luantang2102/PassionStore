using PassionStore.Core.Models;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface ISizeRepository
    {
        Task<Size?> GetByIdAsync(Guid sizeId);
        IQueryable<Size> GetAllAsync();
        Task<Size> CreateAsync(Size size);
        Task UpdateAsync(Size size);
        Task DeleteAsync(Size size);
    }
}