using Microsoft.OpenApi.Models;

namespace PassionStore.Api.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Passion Store API", Version = "v1" });

                // Document common responses
                options.OperationFilter<SwaggerResponseFilter>();
            });

            return services;
        }
    }
}
