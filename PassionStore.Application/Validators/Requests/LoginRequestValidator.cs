using FluentValidation;
using PassionStore.Application.DTOs.Identities;

namespace PassionStore.Application.Validators.Requests
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(l => l.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

            RuleFor(l => l.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }
}