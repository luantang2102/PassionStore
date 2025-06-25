using PassionStore.Core.Models;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IBrandRepository
    {
        Task<Brand?> GetByIdAsync(Guid brandId);
        IQueryable<Brand> GetAllAsync();
        Task<Brand> CreateAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(Brand brand);
    }
}