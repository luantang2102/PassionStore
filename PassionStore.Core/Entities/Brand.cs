using PassionStore.Core.Entities;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class Brand : BaseEntity
    {
        public required string Name { get; set; }
        public required string Description { get; set; }

        // Navigation property
        public virtual ICollection<Product> Products { get; set; } = [];
    }

}