using FluentValidation;
using Microsoft.AspNetCore.Http;
using PassionStore.Application.DTOs.Users;

namespace PassionStore.Application.Validators
{
    public class UserRequestValidator : AbstractValidator<UserRequest>
    {
        public UserRequestValidator()
        {
            RuleFor(u => u.Image)
                .Must(image => image == null || IsValidImage(image))
                .WithMessage("Image must be a valid image file (JPEG, PNG, GIF) with a maximum size of 5MB.");

            RuleFor(u => u.Gender)
                .MaximumLength(20).WithMessage("Gender cannot exceed 20 characters.")
                .When(u => !string.IsNullOrEmpty(u.Gender));

            RuleFor(u => u.DateOfBirth)
                .Must(dob => dob == null || (dob <= DateTime.Today && dob >= DateTime.Today.AddYears(-120)))
                .WithMessage("Date of birth must be a valid date, not in the future, and not older than 120 years.");
        }

        private bool IsValidImage(IFormFile image)
        {
            if (image == null) return true;

            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            var maxSizeInBytes = 5 * 1024 * 1024; // 5MB

            return allowedContentTypes.Contains(image.ContentType) && image.Length <= maxSizeInBytes;
        }
    }
}