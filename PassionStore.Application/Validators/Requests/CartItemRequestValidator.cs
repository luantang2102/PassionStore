using FluentValidation;
using PassionStore.Application.DTOs.Carts;

namespace PassionStore.Application.Validators.Requests
{
    public class CartItemRequestValidator : AbstractValidator<CartItemRequest>
    {
        public CartItemRequestValidator()
        {
            RuleFor(c => c.ProductVariantId)
                .NotEmpty().WithMessage("ProductVariantId is required.");

            RuleFor(c => c.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100.");
        }
    }
}