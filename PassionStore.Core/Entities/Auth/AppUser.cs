using PassionStore.Core.Entities;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models.Auth
{
    public enum UserRole
    {
        User = 1,
        Admin = 2
    }

    public class AppUser : BaseUser
    {
        public override Guid Id { get; set; }
        public override string? UserName { get; set; } = "Anonymous";
        public override string? Email { get; set; } = "Anonymous";
        public string Gender { get; set; } = "Unknown";
        public DateTime? DateOfBirth { get; set; } 
        public string? ImageUrl { get; set; } = string.Empty;
        public string? PublicId { get; set; } = string.Empty;

        // Navigation properties
        public virtual Cart Cart { get; set; } = null!;
        public virtual ICollection<UserProfile> UserProfiles { get; set; } = [];
        public virtual ICollection<Rating> Ratings { get; set; } = [];
        public virtual ICollection<VerifyCode> VerifyCodes { get; set; } = [];
        public virtual ICollection<Report> Reports { get; set; } = [];
        public virtual ICollection<History> Histories { get; set; } = [];
        public virtual ICollection<Notification> Notifications { get; set; } = [];
        public virtual ICollection<Chat> Chats { get; set; } = [];


    }
}
