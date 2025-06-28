using PassionStore.Application.DTOs.Colors;
using PassionStore.Application.DTOs.Sizes;
using System;
using System.Collections.Generic;

namespace PassionStore.Application.DTOs.Orders
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public string Status { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string ShippingMethod { get; set; } = null!;
        public string? PaymentLink { get; set; }
        public string? PaymentTransactionId { get; set; }
        public Guid UserProfileId { get; set; }
        public string UserFullName { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public string Note { get; set; } = null!;
        public List<OrderItemResponse> OrderItems { get; set; } = [];
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}