using PassionStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);
        Task DeleteAsync(Message message);
        IQueryable<Message> GetAll();
        Task<Message> GetByIdAsync(Guid id);
        Task UpdateAsync(Message message);
    }
}
