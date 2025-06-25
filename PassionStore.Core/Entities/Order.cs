using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Entities
{
    public class Order : BaseEntity
    {
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public string ShippingAddress { get; set; } = null!;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = string.Empty;

        // Foreign key
        public Guid UserProfileId { get; set; }

        // Navigation properties
        public virtual UserProfile UserProfile { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];

    }
}
