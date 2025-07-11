using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data;

namespace PassionStore.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _context;

        public ChatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Chat> GetByIdAsync(Guid id)
        {
            return await _context.Chats
                .Include(c => c.User)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new InvalidOperationException($"Chat with ID {id} not found.");
        }

        public IQueryable<Chat> GetAll()
        {
            return _context.Chats
                .Include(c => c.User)
                .Include(c => c.Messages)
                .AsQueryable();
        }

        public async Task AddAsync(Chat chat)
        {
            await _context.Chats.AddAsync(chat);
        }

        public async Task UpdateAsync(Chat chat)
        {
            _context.Chats.Update(chat);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Chat chat)
        {
            _context.Chats.Remove(chat);
            await Task.CompletedTask;
        }
    }
}