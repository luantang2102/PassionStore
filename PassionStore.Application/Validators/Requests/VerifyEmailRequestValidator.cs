using FluentValidation;
using PassionStore.Application.DTOs.Identities;

namespace PassionStore.Application.Validators.Requests
{
    public class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequest>
    {
        public VerifyEmailRequestValidator()
        {
            RuleFor(v => v.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

            RuleFor(v => v.Code)
                .NotEmpty().WithMessage("Verification code is required.")
                .MaximumLength(10).WithMessage("Verification code cannot exceed 10 characters.");
        }
    }
}