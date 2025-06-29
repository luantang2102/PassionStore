using FluentValidation;
using PassionStore.Application.DTOs.Orders;
using PassionStore.Core.Enums;

namespace PassionStore.Application.Validators.Requests
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        public OrderRequestValidator()
        {
            RuleFor(o => o.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method. Must be COD or PayOS.");

            RuleFor(o => o.ShippingMethod)
                .IsInEnum().WithMessage("Invalid shipping method. Must be Standard, Express, or SameDay.");

            RuleFor(o => o.Note)
                .MaximumLength(1000).WithMessage("Order note cannot exceed 1000 characters.");
        }
    }
}