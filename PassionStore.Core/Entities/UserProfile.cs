using PassionStore.Core.Models.Auth;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class UserProfile : BaseEntity
    {
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }

        // Foreign keys
        public Guid AddressId { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties
        public virtual Address Address { get; set; } = null!;
        public virtual AppUser User { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; } = [];
    }

}