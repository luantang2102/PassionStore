namespace PassionStore.Application.DTOs.Notifications
{
    public class NotificationRequest
    {
        public string UserId { get; set; }
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Content { get; set; }
    }
}