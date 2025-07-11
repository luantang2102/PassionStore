using PassionStore.Core.Enums;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Entities
{
    public class Order : BaseEntity
    {
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;
        public string ShippingAddress { get; set; } = null!;
        public ShippingMethod ShippingMethod { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentLink { get; set; } // PayOS payment link
        public string? PaymentTransactionId { get; set; } // PayOS transaction ID
        public string? ReturnReason { get; set; }
        public string Note { get; set; } = string.Empty;

        // Foreign key
        public Guid UserProfileId { get; set; }

        // Navigation properties
        public virtual UserProfile UserProfile { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];
    }
}