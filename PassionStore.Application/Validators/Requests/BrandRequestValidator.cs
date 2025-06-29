using FluentValidation;
using PassionStore.Application.DTOs.Brands;

namespace PassionStore.Application.Validators.Requests
{
    public class BrandRequestValidator : AbstractValidator<BrandRequest>
    {
        public BrandRequestValidator()
        {
            RuleFor(b => b.Name)
                .NotEmpty().WithMessage("Brand name is required.")
                .MaximumLength(100).WithMessage("Brand name cannot exceed 100 characters.");

            RuleFor(b => b.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
        }
    }
}