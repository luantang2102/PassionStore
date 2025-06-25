
namespace PassionStore.Application.DTOs.UserProfiles
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public required string PhoneNumber { get; set; }
        public required string FullName { get; set; }
        public required string Province { get; set; }
        public required string District { get; set; }
        public required string Ward { get; set; }
        public required string SpecificAddress { get; set; }
        public required bool IsDefault { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        //public Guid? UserId { get; set; }
        //public string? UserName { get; set; }
        //public string? Email { get; set; }

    }
}
