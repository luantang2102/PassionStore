namespace PassionStore.Application.DTOs.Carts
{
    public class CartResponse
    {
        public Guid Id { get; set; }
        public List<CartItemResponse> CartItems { get; set; } = [];
    }
}
