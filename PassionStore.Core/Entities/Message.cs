using PassionStore.Core.Models.Base;

namespace PassionStore.Core.Models
{
    public class Message : BaseEntity
    {
        public string Content { get; set; } = string.Empty; public bool IsUserMessage { get; set; } = true;

        // Foreign key
        public Guid ChatId { get; set; }

        // Navigation property
        public virtual Chat Chat { get; set; } = null!;
    }

}