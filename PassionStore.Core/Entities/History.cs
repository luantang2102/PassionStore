using PassionStore.Core.Models.Auth;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class History : BaseEntity
    {
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; } = string.Empty; public string Action { get; set; } = string.Empty;

        // Foreign key
        public Guid UserId { get; set; }

        // Navigation property
        public virtual AppUser User { get; set; } = null!;
    }

}