using PassionStore.Core.Models;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IChatRepository
    {
        Task AddAsync(Chat chat);
        Task DeleteAsync(Chat chat);
        IQueryable<Chat> GetAll();
        Task<Chat> GetByIdAsync(Guid id);
        Task UpdateAsync(Chat chat);
    }
}
