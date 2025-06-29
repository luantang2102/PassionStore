using FluentValidation;
using PassionStore.Application.DTOs.Colors;
using System.Text.RegularExpressions;

namespace PassionStore.Application.Validators.Requests
{
    public class ColorRequestValidator : AbstractValidator<ColorRequest>
    {
        public ColorRequestValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Color name is required.")
                .MaximumLength(50).WithMessage("Color name cannot exceed 50 characters.");

            RuleFor(c => c.HexCode)
                .NotEmpty().WithMessage("Hex code is required.")
                .Matches(new Regex("^#[0-9A-Fa-f]{6}$")).WithMessage("Hex code must be a valid 6-digit hexadecimal color code (e.g., #FF0000).");
        }
    }
}