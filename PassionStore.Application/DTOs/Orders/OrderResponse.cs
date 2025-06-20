using PassionStore.Application.DTOs.Addresses;

namespace PassionStore.Application.DTOs.Orders
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public Guid UserProfileId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public Guid ShippingAddressId { get; set; }
        public AddressResponse ShippingAddress { get; set; } = null!;
        public List<OrderItemResponse> OrderItems { get; set; } = [];
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
