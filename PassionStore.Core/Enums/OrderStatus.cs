namespace PassionStore.Core.Enums
{
    public enum OrderStatus
    {
        PendingPayment,
        PaymentConfirmed,
        PaymentFailed,
        OrderConfirmed,
        Processing,
        ReadyToShip,
        Shipped,
        OutForDelivery,
        Delivered,
        PaymentReceived, // For COD
        ReturnRequested,
        Returned,
        Refunded,
        Completed,
        OnHold,
        Cancelled
    }
}