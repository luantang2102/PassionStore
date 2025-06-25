namespace PassionStore.Application.DTOs.Orders
{
    public class OrderRequest
    {
        public Guid ShippingAddressId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
