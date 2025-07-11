using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassionStore.Application.DTOs.Chats
{
    public class ChatResponse
    {
        public Guid Id { get; set; }
        public string Topic { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
