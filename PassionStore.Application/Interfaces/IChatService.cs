using PassionStore.Application.DTOs.Chats;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;
using PassionStore.Core.Models;
using PassionStore.Core.Models.Auth;
using System.Linq;

namespace PassionStore.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatResponse> CreateChatAsync(ChatRequest chatRequest, Guid userId);
        Task<PagedList<MessageResponse>> GetMessagesAsync(Guid chatId, MessageParams messageParams, Guid currentUserId);
        Task<MessageResponse> SendMessageAsync(MessageRequest messageRequest, Guid chatId, Guid userId);
        Task<AppUser> GetUserByIdAsync(Guid userId);
        Task<bool> IsUserInRoleAsync(Guid userId, string role);
        IQueryable<Chat> GetChatsQuery();
    }
}