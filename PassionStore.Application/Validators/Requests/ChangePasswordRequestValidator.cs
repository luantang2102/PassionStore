using FluentValidation;
using PassionStore.Application.DTOs.Identities;

namespace PassionStore.Application.Validators.Requests
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(c => c.OldPassword)
                .NotEmpty().WithMessage("Old password is required.")
                .MinimumLength(6).WithMessage("Old password must be at least 6 characters long.");

            RuleFor(c => c.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters long.")
                .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("New password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one special character.")
                .NotEqual(c => c.OldPassword).WithMessage("New password must not be the same as the old password.");
        }
    }
}