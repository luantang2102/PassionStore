namespace PassionStore.Application.DTOs.Notifications
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}