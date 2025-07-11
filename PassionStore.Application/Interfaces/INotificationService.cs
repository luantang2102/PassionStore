using PassionStore.Application.DTOs.Notifications;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;

namespace PassionStore.Application.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResponse> CreateNotificationAsync(Guid userId, Guid objectId, string objectType, string content);
        Task<PagedList<NotificationResponse>> GetNotificationsAsync(Guid userId, NotificationParams notificationParams);
        Task MarkNotificationAsReadAsync(Guid notificationId, Guid userId);
    }
}