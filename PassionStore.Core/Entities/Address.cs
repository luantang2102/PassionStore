using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class Address : BaseEntity
    {
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }


        // Navigation properties
        public virtual ICollection<UserProfile> UserProfiles { get; set; } = [];
        public virtual ICollection<Order> Orders { get; set; } = [];
    }

}