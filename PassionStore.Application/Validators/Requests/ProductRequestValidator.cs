using FluentValidation;
using Microsoft.AspNetCore.Http;
using PassionStore.Application.DTOs.Products;

namespace PassionStore.Application.Validators.Requests
{
    public class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidator()
        {
            RuleFor(product => product.Name)
                .NotEmpty().WithMessage("Product name is required.");

            RuleFor(product => product.Description)
                .NotEmpty().WithMessage("Product description is required.");

            //RuleFor(product => product.Price)
            //    .GreaterThan(0).WithMessage("Price must be greater than 0.");

            //RuleFor(product => product.StockQuantity)
            //    .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");

            RuleForEach(product => product.Images)
                .SetValidator(new ExistingProductImageRequestValidator())
                .When(product => product.Images != null && product.Images.Count != 0);

            RuleFor(product => product.FormImages)
                .Must(files => files == null || files.All(file => file != null && file.Length > 0))
                .WithMessage("All uploaded images must have content.")
                .Must(files => files == null || files.All(file => file.Length <= 5 * 1024 * 1024)) // Max 5 MB per file
                .WithMessage("Each image must be less than 5 MB.")
                .Must(files => files == null || files.All(file => IsValidImageType(file)))
                .WithMessage("Only JPEG, PNG, and GIF images are allowed.");

            RuleFor(product => product.BrandId)
                .NotEmpty().WithMessage("Brand ID is required.")
                .Must(brandId => brandId != Guid.Empty).WithMessage("Brand ID cannot be empty.");

            RuleFor(product => product.CategoryIds)
                .NotEmpty().WithMessage("At least one category ID is required.")
                .Must(categoryIds => categoryIds.All(id => id != Guid.Empty))
                .WithMessage("Category IDs cannot contain empty GUIDs.");

            RuleFor(product => product.IsFeatured)
                .NotNull().WithMessage("IsFeatured must be specified.")
                .Must(isFeatured => isFeatured == true || isFeatured == false)
                .WithMessage("IsFeatured must be a boolean value.");

            RuleFor(product => product.IsNotHadVariants)
                .NotNull().WithMessage("IsNotHadVariants must be specified.")
                .Must(isNotHadVariants => isNotHadVariants == true || isNotHadVariants == false)
                .WithMessage("IsNotHadVariants must be a boolean value.");

            RuleFor(product => product.DefaultVariantPrice)
                .GreaterThan(0).WithMessage("Default variant price must be greater than 0.")
                .When(product => product.IsNotHadVariants);

            RuleFor(product => product.DefaultVariantStockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Default variant stock quantity cannot be negative.")
                .When(product => product.IsNotHadVariants);
        }

        private bool IsValidImageType(IFormFile file)
        {
            if (file == null) return true;
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            return allowedTypes.Contains(file.ContentType);
        }
    }

    public class ExistingProductImageRequestValidator : AbstractValidator<ExistingProductImageRequest>
    {
        public ExistingProductImageRequestValidator()
        {
            RuleFor(image => image.Id)
                .NotEmpty().WithMessage("Existing image ID is required.");

        }
    }
}