using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models.Auth;
using PassionStore.Infrastructure.Data;
using PassionStore.Infrastructure.Externals;
using PassionStore.Infrastructure.Externals.Payos;
using PassionStore.Infrastructure.Repositories;
using PassionStore.Infrastructure.Settings;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Read Configuration Options From AppSettings
        var connectionStringsOption = new ConnectionStringsOption();
        configuration.GetSection("InfrastructureSettings:ConnectionStringsOption").Bind(connectionStringsOption);

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionStringsOption.Default));

        services.AddIdentity<AppUser, IdentityRole<Guid>>(opt =>
        {
            opt.Password.RequireDigit = true;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireNonAlphanumeric = true;
            opt.Password.RequireUppercase = false;
            opt.Password.RequiredLength = 6;
            opt.Password.RequiredUniqueChars = 1;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // Dependencies Services, Repos
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IRatingRepository, RatingRepository>();
        services.AddScoped<IHelpfulVoteRepository, HelpfulVoteRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartItemRepository, CartItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IVerifyCodeRepository, VerifyCodeRepository>();
        services.AddScoped<IColorRepository, ColorRepository>();
        services.AddScoped<ISizeRepository, SizeRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        var emailOption = new EmailOption();
        configuration.GetSection("InfrastructureSettings:EmailOption").Bind(emailOption);
        services.AddScoped<EmailService>(_ =>
        {
            return new EmailService(emailOption);
        });

        var cloudinaryOption = new CloudinaryOption();
        configuration.GetSection("InfrastructureSettings:CloudinaryOption").Bind(cloudinaryOption);
        services.AddScoped<CloudinaryService>(_ =>
        {
            return new CloudinaryService(cloudinaryOption);
        });

        var payOSOption = new PayOSOption();
        configuration.GetSection("InfrastructureSettings:PayOSOption").Bind(payOSOption);
        services.AddScoped<PayOSService>(_ =>
        {
            return new PayOSService(payOSOption);
        });

        return services;
    }
}
