namespace PassionStore.Application.DTOs.Orders
{
    public class PayOSWebhookRequest
    {
        public Guid OrderId { get; set; }
        public long OrderCode { get; set; }
    }
}