using FluentValidation;
using PassionStore.Application.DTOs.Identities;

namespace PassionStore.Application.Validators.Requests
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(r => r.UserName)
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.")
                .When(r => !string.IsNullOrEmpty(r.UserName));

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(r => r.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(r => r.Password).WithMessage("Confirm password must match password.");

            RuleFor(r => r.ImageUrl)
                .MaximumLength(1000).WithMessage("Image URL cannot exceed 1000 characters.")
                .When(r => !string.IsNullOrEmpty(r.ImageUrl));

            RuleFor(r => r.PublicId)
                .MaximumLength(100).WithMessage("Public ID cannot exceed 100 characters.")
                .When(r => !string.IsNullOrEmpty(r.PublicId));
        }
    }
}