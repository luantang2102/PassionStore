using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassionStore.Application.DTOs.Orders
{
    public class OrderRequest
    {
        public Guid ShippingAddressId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
