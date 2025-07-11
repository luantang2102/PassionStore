using PassionStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task DeleteAsync(Notification notification);
        IQueryable<Notification> GetAll();
        Task<Notification> GetByIdAsync(Guid id);
        Task UpdateAsync(Notification notification);
    }
}
