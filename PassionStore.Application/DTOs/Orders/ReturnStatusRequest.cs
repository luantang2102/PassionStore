using PassionStore.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassionStore.Application.DTOs.Orders
{
    public class ReturnStatusRequest
    {
        public OrderStatus Status { get; set; }
        public string? RefundReason { get; set; }
    }
}
