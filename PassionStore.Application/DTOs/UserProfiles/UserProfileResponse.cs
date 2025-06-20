using PassionStore.Application.DTOs.Addresses;

namespace PassionStore.Application.DTOs.UserProfiles
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public required string PhoneNumber { get; set; }
        public required AddressResponse Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        //public Guid? UserId { get; set; }
        //public string? UserName { get; set; }
        //public string? Email { get; set; }

    }
}
