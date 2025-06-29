using FluentValidation;
using PassionStore.Application.DTOs.Identities;

namespace PassionStore.Application.Validators.Requests
{
    public class EmailRequestValidator : AbstractValidator<EmailRequest>
    {
        public EmailRequestValidator()
        {
            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");
        }
    }
}