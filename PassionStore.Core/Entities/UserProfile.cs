using PassionStore.Core.Models.Auth;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Entities
{
    public class UserProfile : BaseEntity
    {
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Province { get; set; }
        public required string District { get; set; }
        public required string Ward { get; set; }
        public required string SpecificAddress { get; set; }
        public required bool IsDefault { get; set; } = false;

        // Foreign keys
        public Guid UserId { get; set; }

        // Navigation properties
        public virtual AppUser User { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; } = [];
    }

}