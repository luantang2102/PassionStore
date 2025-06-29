using PassionStore.Core.Models.Auth;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Entities
{
    public class HelpfulVote : BaseEntity
    {
        public Guid RatingId { get; set; }
        public Guid UserId { get; set; }

        public Rating Rating { get; set; } = null!;
        public AppUser User { get; set; } = null!;
    }
}