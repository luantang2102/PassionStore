using PassionStore.Core.Models.Auth;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class Report : BaseEntity
    {
        public string Content { get; set; } = string.Empty; public Guid ObjectId { get; set; }
        public string ObjectType { get; set; } = string.Empty; public bool Processed { get; set; } = false;

        // Foreign key
        public Guid SenderId { get; set; }

        // Navigation property
        public virtual AppUser Sender { get; set; } = null!;
    }

}