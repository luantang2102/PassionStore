using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PassionStore.Api.Filters;
using PassionStore.Infrastructure.Settings;
using System.Text;

namespace PassionStore.Api.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddCustomJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOption = new JwtOption();
            configuration.GetSection("InfrastructureSettings:JwtOption").Bind(jwtOption);
            var key = Encoding.UTF8.GetBytes(jwtOption.Key);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOption.Issuer,
                    ValidAudience = jwtOption.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                opt.Events = JwtBearerEventsFilter.CreateJwtBearerEvents();
            });

            return services;
        }
    }
}
