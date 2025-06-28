using PassionStore.Core.Enums;

namespace PassionStore.Application.DTOs.Orders
{
    public class OrderRequest
    {
        public PaymentMethod PaymentMethod { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}