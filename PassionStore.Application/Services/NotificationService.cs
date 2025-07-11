using Microsoft.AspNetCore.SignalR;
using PassionStore.Application.Interfaces;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Application.Hubs;
using PassionStore.Application.Paginations;
using PassionStore.Core.Exceptions;
using PassionStore.Application.DTOs.Notifications;
using PassionStore.Application.Helpers.Params;
using PassionStore.Infrastructure.Extensions;

namespace PassionStore.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(
            IHubContext<ChatHub> hubContext,
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork)
        {
            _hubContext = hubContext;
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<NotificationResponse> CreateNotificationAsync(Guid userId, Guid objectId, string objectType, string content)
        {
            var notification = new Notification
            {
                UserId = userId,
                ObjectId = objectId,
                ObjectType = objectType,
                Content = content,
                IsRead = false,
                CreatedDate = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.CommitAsync();

            // Send real-time notification
            await _hubContext.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", objectId.ToString(), content);

            return new NotificationResponse
            {
                Id = notification.Id,
                UserId = notification.UserId,
                ObjectId = notification.ObjectId,
                ObjectType = notification.ObjectType,
                Content = notification.Content,
                IsRead = notification.IsRead,
                CreatedDate = notification.CreatedDate
            };
        }

        public async Task<PagedList<NotificationResponse>> GetNotificationsAsync(Guid userId, NotificationParams notificationParams)
        {
            var query = _notificationRepository.GetAll()
                .Where(n => n.UserId == userId)
                .Filter(notificationParams.IsRead)
                .Sort(notificationParams.OrderBy)
                .Select(n => new NotificationResponse
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    ObjectId = n.ObjectId,
                    ObjectType = n.ObjectType,
                    Content = n.Content,
                    IsRead = n.IsRead,
                    CreatedDate = n.CreatedDate
                });

            return await PaginationService.ToPagedList(query, notificationParams.PageNumber, notificationParams.PageSize);
        }

        public async Task MarkNotificationAsReadAsync(Guid notificationId, Guid userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null)
            {
                throw new AppException(ErrorCode.NOTIFICATION_NOT_FOUND, new Dictionary<string, object> { { "NotificationId", notificationId.ToString() } });
            }

            if (notification.UserId != userId)
            {
                throw new AppException(ErrorCode.ACCESS_DENIED, new Dictionary<string, object> { { "NotificationId", notificationId.ToString() }, { "UserId", userId.ToString() } });
            }

            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification);
            await _unitOfWork.CommitAsync();
        }
    }
}