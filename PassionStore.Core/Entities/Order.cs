using PassionStore.Core.Entities;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class Order : BaseEntity
    {
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = string.Empty;

        // Foreign key
        public Guid UserProfileId { get; set; }
        public Guid ShippingAddressId { get; set; }

        // Navigation properties
        public virtual UserProfile UserProfile { get; set; } = null!;
        public virtual Address ShippingAddress { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];

    }
}
