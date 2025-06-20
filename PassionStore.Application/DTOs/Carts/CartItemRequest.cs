namespace PassionStore.Application.DTOs.Carts
{
    public class CartItemRequest
    {
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}
