using FluentValidation;
using PassionStore.Application.DTOs.UserProfiles;
using System.Text.RegularExpressions;

namespace PassionStore.Application.Validators
{
    public class UserProfileRequestValidator : AbstractValidator<UserProfileRequest>
    {
        public UserProfileRequestValidator()
        {
            RuleFor(u => u.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

            RuleFor(u => u.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(new Regex(@"^\+?\d{10,15}$")).WithMessage("Phone number must be a valid number with 10 to 15 digits.")
                .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters.");

            RuleFor(u => u.Province)
                .NotEmpty().WithMessage("Province is required.")
                .MaximumLength(100).WithMessage("Province cannot exceed 100 characters.");

            RuleFor(u => u.District)
                .NotEmpty().WithMessage("District is required.")
                .MaximumLength(100).WithMessage("District cannot exceed 100 characters.");

            RuleFor(u => u.Ward)
                .NotEmpty().WithMessage("Ward is required.")
                .MaximumLength(100).WithMessage("Ward cannot exceed 100 characters.");

            RuleFor(u => u.SpecificAddress)
                .NotEmpty().WithMessage("Specific address is required.")
                .MaximumLength(200).WithMessage("Specific address cannot exceed 200 characters.");

            RuleFor(u => u.IsDefault)
                .NotNull().WithMessage("IsDefault must be specified.");
        }
    }
}