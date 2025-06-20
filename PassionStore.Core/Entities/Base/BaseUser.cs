using Microsoft.AspNetCore.Identity;

namespace PassionStore.Core.Models.Base
{
    public class BaseUser : IdentityUser<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
