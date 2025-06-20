using PassionStore.Core.Models.Auth;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class VerifyCode : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false;
        public DateTime ExpiryTime { get; set; }

        // Foreign key
        public Guid UserId { get; set; }

        // Navigation property
        public virtual AppUser User { get; set; } = null!;
    }

}