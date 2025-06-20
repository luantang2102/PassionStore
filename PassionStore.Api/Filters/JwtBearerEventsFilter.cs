using Microsoft.AspNetCore.Authentication.JwtBearer;
using PassionStore.Core.Exceptions;

namespace PassionStore.Api.Filters
{
    public class JwtBearerEventsFilter
    {
        public static JwtBearerEvents CreateJwtBearerEvents()
        {
            return new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies["auth_jwt"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Token = token;
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    throw new AccessDeniedException(ErrorCode.UNAUTHORIZED_ACCESS);
                },
                OnForbidden = context =>
                {
                    throw new AccessDeniedException(ErrorCode.ACCESS_DENIED);
                }
            };
        }
    }
}
