using FluentValidation;
using PassionStore.Application.DTOs.Ratings;

namespace PassionStore.Application.Validators.Requests
{
    public class RatingRequestValidator : AbstractValidator<RatingRequest>
    {
        public RatingRequestValidator()
        {
            RuleFor(r => r.Value)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating value must be between 1 and 5.");

            RuleFor(r => r.Comment)
                .MaximumLength(500)
                .WithMessage("Comment cannot exceed 500 characters.");

            RuleFor(r => r.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.");
        }
    }
}