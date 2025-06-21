using PassionStore.Core.Entities;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class ProductImage : BaseEntity
    {
        public required string ImageUrl { get; set; }
        public required string PublicId { get; set; }
        public int Order { get; set; }

        // Foreign key
        public Guid ProductId { get; set; }

        // Navigation property
        public virtual Product Product { get; set; } = null!;
    }
}