using PassionStore.Core.Models.Auth;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class Notification : BaseEntity
    {
        public string Content { get; set; } = string.Empty; public Guid ObjectId { get; set; }
        public string ObjectType { get; set; } = string.Empty; public bool IsRead { get; set; } = false;

        // Foreign key
        public Guid UserId { get; set; }

        // Navigation property
        public virtual AppUser User { get; set; } = null!;
    }

}