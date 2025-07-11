using AssetManagement.Application.Services.Auth;
using Microsoft.Extensions.Configuration;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Interfaces.Auth;
using PassionStore.Application.Services;
using PassionStore.Application.Services.Auth;
using PassionStore.Core.Interfaces.IServices;
using PassionStore.Infrastructure.Settings;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            var jwtOption = new JwtOption();
            configuration.GetSection("InfrastructureSettings:JwtOption").Bind(jwtOption);

            services.AddScoped<ITokenService, JwtService>(_ =>
            {
                return new JwtService(jwtOption);
            });
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductVariantService, ProductVariantService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IColorService, ColorService>();
            services.AddScoped<ISizeService, SizeService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
