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
                OnChallenge = async context =>
                {
                    // Prevent the default challenge response (e.g., redirect to login)
                    context.HandleResponse();

                    // Set the response status code and content
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    // Create the error response
                    var errorResponse = new
                    {
                        Code = ErrorCode.UNAUTHORIZED_ACCESS.GetCode(),
                        Message = ErrorCode.UNAUTHORIZED_ACCESS.GetMessage()
                    };

                    // Write the response
                    await context.Response.WriteAsJsonAsync(errorResponse);
                },
                OnForbidden = async context =>
                {
                    // Set the response status code and content for forbidden
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    // Create the error response
                    var errorResponse = new
                    {
                        Code = ErrorCode.ACCESS_DENIED.GetCode(),
                        Message = ErrorCode.ACCESS_DENIED.GetMessage()
                    };

                    // Write the response
                    await context.Response.WriteAsJsonAsync(errorResponse);
                }
            };
        }
    }
}