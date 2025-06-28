using System;

namespace PassionStore.Core.Enums
{
    public enum OrderStatus
    {
        PendingPayment = 0,
        PaymentConfirmed,
        OrderConfirmed,
        Processing,
        ReadyToShip,
        Shipped,
        OutForDelivery,
        Delivered,
        PaymentReceived,
        Completed,
        PaymentFailed,
        OnHold,
        Cancelled,
        Returned,
        Refunded
    }
}