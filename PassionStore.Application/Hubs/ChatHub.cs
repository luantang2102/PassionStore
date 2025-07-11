using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PassionStore.Core.Models;
using System.Security.Claims;
using PassionStore.Core.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace PassionStore.Application.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUserRepository _userRepository;

        public ChatHub(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // Add user to their own group for notifications
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);

                // Add user to their active chats
                var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
                if (user != null)
                {
                    var isAdmin = await _userRepository.IsInRoleAsync(user, "Admin");
                    var chats = await _userRepository.GetAllAsync()
                        .Where(u => u.Id == Guid.Parse(userId))
                        .SelectMany(u => u.Chats)
                        .ToListAsync();

                    foreach (var chat in chats)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, chat.Id.ToString());
                    }

                    // Admins join a global admin group for user-initiated chats
                    if (isAdmin)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                    }
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // Remove user from their own group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

                // Remove user from their active chats
                var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
                if (user != null)
                {
                    var isAdmin = await _userRepository.IsInRoleAsync(user, "Admin");
                    var chats = await _userRepository.GetAllAsync()
                        .Where(u => u.Id == Guid.Parse(userId))
                        .SelectMany(u => u.Chats)
                        .ToListAsync();

                    foreach (var chat in chats)
                    {
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chat.Id.ToString());
                    }

                    if (isAdmin)
                    {
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
                    }
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(Guid chatId, string content)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("User not authenticated.");
            }

            // Broadcast the message to the chat group (user and admins)
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", chatId, userId, content);

            // If the sender is an admin, send a notification to the user
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user != null && await _userRepository.IsInRoleAsync(user, "Admin"))
            {
                var chat = await _userRepository.GetAllAsync()
                    .SelectMany(u => u.Chats)
                    .FirstOrDefaultAsync(c => c.Id == chatId);
                if (chat != null)
                {
                    await Clients.Group(chat.UserId.ToString()).SendAsync("ReceiveNotification", chatId, $"New message in chat: {content.Substring(0, Math.Min(content.Length, 50))}...");
                }
            }
        }
    }
}