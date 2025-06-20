using PassionStore.Core.Entities;
using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class Category : BaseEntity
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Level { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // Foreign key
        public Guid? ParentCategoryId { get; set; }

        // Navigation property
        public virtual Category ParentCategory { get; set; } = null!;
        public virtual ICollection<Category> SubCategories { get; set; } = [];
        public virtual ICollection<Product> Products { get; set; } = [];

    }
}
