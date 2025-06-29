using FluentValidation;
using PassionStore.Application.DTOs.Orders;
using PassionStore.Core.Enums;

namespace PassionStore.Application.Validators.Requests
{
    public class OrderStatusRequestValidator : AbstractValidator<OrderStatusRequest>
    {
        public OrderStatusRequestValidator()
        {
            RuleFor(o => o.Status)
                .IsInEnum().WithMessage("Invalid order status. Must be a valid order status value.");
        }
    }
}