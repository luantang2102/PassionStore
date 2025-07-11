using Microsoft.AspNetCore.SignalR;
using PassionStore.Application.DTOs.Chats;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Hubs;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Paginations;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Core.Models.Auth;
using PassionStore.Infrastructure.Data;
using PassionStore.Infrastructure.Extensions;

namespace PassionStore.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChatRepository _chatRepository;
        private readonly INotificationService _notificationService;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(
            IHubContext<ChatHub> hubContext,
            IChatRepository chatRepository,
            INotificationService notificationService,
            IMessageRepository messageRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _hubContext = hubContext;
            _chatRepository = chatRepository;
            _notificationService = notificationService;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ChatResponse> CreateChatAsync(ChatRequest chatRequest, Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND, new Dictionary<string, object> { { "UserId", userId.ToString() } });
            }

            if (await _userRepository.IsInRoleAsync(user, "Admin"))
            {
                throw new AppException(ErrorCode.ACCESS_DENIED, new Dictionary<string, object> { { "UserId", userId.ToString() }, { "Message", "Admins cannot initiate chats with themselves." } });
            }

            var chat = new Chat
            {
                Topic = chatRequest.Topic,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            await _chatRepository.AddAsync(chat);
            await _unitOfWork.CommitAsync();

            // Notify admins of new chat
            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNewChat", chat.Id, chat.Topic, userId);

            return new ChatResponse
            {
                Id = chat.Id,
                Topic = chat.Topic,
                UserId = chat.UserId,
                CreatedDate = chat.CreatedDate
            };
        }

        public async Task<PagedList<MessageResponse>> GetMessagesAsync(Guid chatId, MessageParams messageParams, Guid currentUserId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat == null)
            {
                throw new AppException(ErrorCode.CHAT_NOT_FOUND, new Dictionary<string, object> { { "ChatId", chatId.ToString() } });
            }

            if (chat.UserId != currentUserId && !await _userRepository.IsInRoleAsync(await _userRepository.GetByIdAsync(currentUserId), "Admin"))
            {
                throw new AppException(ErrorCode.ACCESS_DENIED, new Dictionary<string, object> { { "ChatId", chatId.ToString() }, { "UserId", currentUserId.ToString() } });
            }

            var query = _messageRepository.GetAll()
                .Filter(chatId, messageParams.IsUserMessage)
                .Search(messageParams.SearchTerm)
                .Sort(messageParams.OrderBy)
                .Select(m => new MessageResponse
                {
                    Id = m.Id,
                    Content = m.Content,
                    IsUserMessage = m.IsUserMessage,
                    ChatId = m.ChatId,
                    CreatedDate = m.CreatedDate
                });

            return await PaginationService.ToPagedList(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<MessageResponse> SendMessageAsync(MessageRequest messageRequest, Guid chatId, Guid userId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat == null)
            {
                throw new AppException(ErrorCode.CHAT_NOT_FOUND, new Dictionary<string, object> { { "ChatId", chatId.ToString() } });
            }

            if (chat.UserId != userId && !await _userRepository.IsInRoleAsync(await _userRepository.GetByIdAsync(userId), "Admin"))
            {
                throw new AppException(ErrorCode.ACCESS_DENIED, new Dictionary<string, object> { { "ChatId", chatId.ToString() }, { "UserId", userId.ToString() } });
            }

            var isAdmin = await _userRepository.IsInRoleAsync(await _userRepository.GetByIdAsync(userId), "Admin");

            var message = new Message
            {
                Content = messageRequest.Content,
                IsUserMessage = !isAdmin,
                ChatId = chatId,
                CreatedDate = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            // Broadcast message to the chat group
            await _hubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", chatId, userId.ToString(), messageRequest.Content);

            // If admin sends a message, notify the user
            if (isAdmin)
            {
                await _notificationService.CreateNotificationAsync(
                    chat.UserId,
                    chatId,
                    "ChatMessage",
                    $"New message in chat '{chat.Topic}': {messageRequest.Content.Substring(0, Math.Min(messageRequest.Content.Length, 50))}..."
                );
            }

            return new MessageResponse
            {
                Id = message.Id,
                Content = message.Content,
                IsUserMessage = message.IsUserMessage,
                ChatId = message.ChatId,
                CreatedDate = message.CreatedDate
            };
        }

        public async Task<AppUser> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId)
                ?? throw new AppException(ErrorCode.USER_NOT_FOUND, new Dictionary<string, object> { { "UserId", userId.ToString() } });
        }

        public async Task<bool> IsUserInRoleAsync(Guid userId, string role)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null && await _userRepository.IsInRoleAsync(user, role);
        }

        public IQueryable<Chat> GetChatsQuery()
        {
            return _chatRepository.GetAll();
        }
    }
}