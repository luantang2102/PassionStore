using PassionStore.Core.Models.Auth;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Entities
{
    public class Rating : BaseEntity
    {
        public int Value { get; set; }
        public string? Comment { get; set; } = string.Empty;

        // Foreign key
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; } = null!;
        public virtual AppUser User { get; set; } = null!;
    }
}
