using AssetManagement.Application.Paginations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.Chats;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Paginations;

namespace PassionStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : BaseApiController
    {
        private readonly IChatService _chatService;

        public ChatsController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetChats([FromQuery] PaginationParams @params)
        {
            var userId = User.GetUserId();
            var user = await _chatService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { code = "USER_NOT_FOUND", message = "User not found." });
            }

            var isAdmin = await _chatService.IsUserInRoleAsync(userId, "Admin");
            var query = _chatService.GetChatsQuery(); // Assume GetChatsQuery in IChatService
            query = isAdmin ? query : query.Where(c => c.UserId == userId);

            var chats = await PaginationService.ToPagedList(
                query.Select(c => new ChatResponse
                {
                    Id = c.Id,
                    Topic = c.Topic,
                    UserId = c.UserId,
                    CreatedDate = c.CreatedDate
                }),
                @params.PageNumber,
                @params.PageSize
            );

            Response.AddPaginationHeader(chats.Metadata);
            return Ok(chats);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateChat([FromBody] ChatRequest chatRequest)
        {
            var userId = User.GetUserId();
            var chat = await _chatService.CreateChatAsync(chatRequest, userId);
            return CreatedAtAction(nameof(GetMessages), new { chatId = chat.Id }, chat);
        }

        [HttpGet("{chatId}/messages")]
        [Authorize]
        public async Task<IActionResult> GetMessages(Guid chatId, [FromQuery] MessageParams messageParams)
        {
            var userId = User.GetUserId();
            var messages = await _chatService.GetMessagesAsync(chatId, messageParams, userId);
            Response.AddPaginationHeader(messages.Metadata);
            return Ok(messages);
        }

        [HttpPost("{chatId}/messages")]
        [Authorize]
        public async Task<IActionResult> SendMessage(Guid chatId, [FromBody] MessageRequest messageRequest)
        {
            var userId = User.GetUserId();
            var message = await _chatService.SendMessageAsync(messageRequest, chatId, userId);
            return Ok(message);
        }
    }
}