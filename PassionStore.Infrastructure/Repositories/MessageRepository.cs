using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data;

namespace PassionStore.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Message> GetByIdAsync(Guid id)
        {
            return await _context.Messages
                .Include(m => m.Chat)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id)
                ?? throw new InvalidOperationException($"Message with ID {id} not found.");
        }

        public IQueryable<Message> GetAll()
        {
            return _context.Messages
                .Include(m => m.Chat)
                .ThenInclude(c => c.User)
                .AsQueryable();
        }

        public async Task AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
        }

        public async Task UpdateAsync(Message message)
        {
            _context.Messages.Update(message);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Message message)
        {
            _context.Messages.Remove(message);
            await Task.CompletedTask;
        }
    }
}