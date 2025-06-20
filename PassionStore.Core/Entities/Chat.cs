using PassionStore.Core.Models.Auth;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class Chat : BaseEntity
    {
        public string Topic { get; set; } = string.Empty;

        // Foreign key
        public Guid UserId { get; set; }

        // Navigation properties
        public virtual AppUser User { get; set; } = null!;
        public virtual ICollection<Message> Messages { get; set; } = [];
    }

}