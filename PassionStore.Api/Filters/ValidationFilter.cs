using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PassionStore.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Check action arguments for validation
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null) continue;

                // Resolve the validator for the argument type
                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    // Perform validation
                    var validationContext = new ValidationContext<object>(argument);
                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (!validationResult.IsValid)
                    {
                        // Add validation errors to ModelState
                        foreach (var error in validationResult.Errors)
                        {
                            context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }
                    }
                }
            }

            // Check ModelState and throw ValidationException if invalid
            if (context.ModelState != null && !context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                    );

                throw new Core.Exceptions.ValidationException(errors);
            }

            await next();
        }
    }
}