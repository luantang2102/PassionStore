using FluentValidation;
using PassionStore.Application.DTOs.Sizes;

namespace PassionStore.Application.Validators
{
    public class SizeRequestValidator : AbstractValidator<SizeRequest>
    {
        public SizeRequestValidator()
        {
            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("Size name is required.")
                .MaximumLength(50).WithMessage("Size name cannot exceed 50 characters.");
        }
    }
}