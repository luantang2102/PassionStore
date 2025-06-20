namespace PassionStore.Shared.DTOs.Request
{
    public class OrderRequest
    {
        public bool SaveAddress { get; set; }
        public ShippingAddressRequest ShippingAddress { get; set; } = null!;
    }
}
