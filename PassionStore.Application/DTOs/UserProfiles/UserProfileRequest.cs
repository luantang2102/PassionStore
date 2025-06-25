namespace PassionStore.Application.DTOs.UserProfiles
{
    public class UserProfileRequest
    {
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Province { get; set; }
        public required string District { get; set; }
        public required string Ward { get; set; }
        public required string SpecificAddress { get; set; }
        public bool IsDefault { get; set; } = false;
    }
}
