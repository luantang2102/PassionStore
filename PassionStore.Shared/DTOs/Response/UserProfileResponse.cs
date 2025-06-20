using System;

namespace PassionStore.Shared.DTOs.Response
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }

    }
}
