using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassionStore.Application.DTOs.Chats
{
    public class MessageResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsUserMessage { get; set; }
        public Guid ChatId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
