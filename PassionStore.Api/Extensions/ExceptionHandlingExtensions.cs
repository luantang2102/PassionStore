using PassionStore.Api.Filters;

namespace PassionStore.Api.Extensions
{
    public static class ExceptionHandlingExtensions
    {
        public static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            return services;
        }
    }
}
