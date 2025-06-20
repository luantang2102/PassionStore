using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Application.DTOs.Errors;
using PassionStore.Core.Exceptions;

namespace PassionStore.Api.Filters
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            return exception switch
            {
                AccessDeniedException accessDeniedException => await HandleAccessDeniedExceptionAsync(httpContext, accessDeniedException, cancellationToken),
                ValidationException validationException => await HandleValidationExceptionAsync(httpContext, validationException, cancellationToken),
                AppException appException => await HandleAppExceptionAsync(httpContext, appException, cancellationToken),
                NotImplementedException notImplementedException => await HandleNotImplementedExceptionAsync(httpContext, notImplementedException, cancellationToken),
                _ => await HandleGenericExceptionAsync(httpContext, exception, cancellationToken)
            };
        }

        private async Task<bool> HandleAppExceptionAsync(HttpContext httpContext, AppException exception, CancellationToken cancellationToken)
        {
            var errorCode = exception.GetErrorCode();
            var attributes = exception.GetAttributes();

            var response = new ErrorResponse(errorCode.GetCode(), errorCode.GetMessage());

            httpContext.Response.StatusCode = errorCode.GetStatus();
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
        private async Task<bool> HandleValidationExceptionAsync(HttpContext httpContext, ValidationException exception, CancellationToken cancellationToken)
        {
            var errorCode = exception.GetErrorCode();
            var response = new ErrorWithBodyResponse<Dictionary<string, object>>(
                errorCode.GetCode(),
                errorCode.GetMessage(),
                exception.GetAttributes());

            httpContext.Response.StatusCode = errorCode.GetStatus();
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }

        private async Task<bool> HandleAccessDeniedExceptionAsync(HttpContext httpContext, AccessDeniedException exception, CancellationToken cancellationToken)
        {
            var errorCode = exception.GetErrorCode();

            var response = new ErrorResponse(
                errorCode.GetCode(),
                errorCode.GetMessage()
            );

            httpContext.Response.StatusCode = errorCode.GetStatus();
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }

        private async Task<bool> HandleNotImplementedExceptionAsync(HttpContext httpContext, NotImplementedException exception, CancellationToken cancellationToken)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status501NotImplemented,
                Title = "Not Implemented",
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };
            httpContext.Response.StatusCode = StatusCodes.Status501NotImplemented;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        private async Task<bool> HandleGenericExceptionAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}
