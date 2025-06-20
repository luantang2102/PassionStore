using PassionStore.Core.Models.Auth;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class Cart : BaseEntity
    {
        // Foreign key
        public Guid UserId { get; set; }

        // Navigation property
        public virtual AppUser User { get; set; } = null!;
        public virtual ICollection<CartItem> CartItems { get; set; } = [];

    }
}
