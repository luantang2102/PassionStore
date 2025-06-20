﻿using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PassionStore.Web.Services.Impl
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<OrderService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("NashApp.Api");
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private bool HasValidJwtToken()
        {
            if (!_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("auth_jwt", out var jwtToken))
            {
                _logger.LogWarning("No auth_jwt cookie found");
                return false;
            }

            if (!jwtToken.Contains("."))
            {
                _logger.LogError("JWT is malformed: {JwtToken}", jwtToken.Substring(0, Math.Min(20, jwtToken.Length)) + "...");
                return false;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);
                var exp = token.ValidTo;

                if (exp < DateTime.UtcNow)
                {
                    _logger.LogWarning("JWT is expired: ValidUntil={ValidUntil}, Now={Now}", exp, DateTime.UtcNow);
                    return false;
                }

                _logger.LogDebug("JWT is valid until: {Expiration}", exp.ToString("o"));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse JWT token");
                return false;
            }
        }

        private async Task<bool> RefreshTokenAsync()
        {
            if (!_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("refresh", out var refreshToken))
            {
                _logger.LogWarning("No refresh token cookie found, cannot refresh token");
                return false;
            }

            try
            {
                _logger.LogInformation("Attempting to refresh token");
                var response = await _httpClient.GetAsync("/api/Auth/refresh-token");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Token refreshed successfully");
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Refresh token failed: StatusCode={StatusCode}, Response={Content}",
                    response.StatusCode, errorContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during token refresh");
                return false;
            }
        }

        private async Task<HttpResponseMessage> ExecuteWithRetryAsync(Func<Task<HttpResponseMessage>> action, string operationName)
        {
            if (!HasValidJwtToken())
            {
                _logger.LogInformation("No valid JWT token found for {Operation}, attempting refresh", operationName);

                if (await RefreshTokenAsync())
                {
                    if (!HasValidJwtToken())
                    {
                        _logger.LogError("Still no valid JWT token after refresh for {Operation}", operationName);
                        throw new UnauthorizedAccessException("Authentication failed");
                    }
                }
                else
                {
                    _logger.LogError("Failed to refresh token for {Operation}", operationName);
                    throw new UnauthorizedAccessException("Authentication failed");
                }
            }

            HttpResponseMessage response;
            try
            {
                response = await action();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during {Operation}", operationName);
                throw;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("{Operation} returned 401 Unauthorized, attempting token refresh", operationName);

                if (await RefreshTokenAsync() && HasValidJwtToken())
                {
                    _logger.LogInformation("Token refreshed, retrying {Operation}", operationName);

                    try
                    {
                        response = await action();

                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            _logger.LogError("Still unauthorized after token refresh for {Operation}", operationName);
                            throw new UnauthorizedAccessException("Authentication failed after token refresh");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception during retry of {Operation} after token refresh", operationName);
                        throw;
                    }
                }
                else
                {
                    _logger.LogError("Failed to refresh token for {Operation}", operationName);
                    throw new UnauthorizedAccessException("Authentication failed");
                }
            }

            return response;
        }

        public async Task<int> CreateOrderAsync(OrderRequest request)
        {
            _logger.LogInformation("Creating order with SaveAddress={SaveAddress}", request.SaveAddress);

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response = await ExecuteWithRetryAsync(async () =>
            {
                return await _httpClient.PostAsync("/api/Orders", content);
            }, "CreateOrderAsync");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<int>>(responseContent);

            _logger.LogDebug("Order created successfully: OrderId={OrderId}", apiResponse.Body);

            return apiResponse.Body;
        }

        public async Task<OrderResponse> GetOrderAsync(Guid orderId)
        {
            _logger.LogInformation("Fetching order: OrderId={OrderId}", orderId);

            var response = await ExecuteWithRetryAsync(async () =>
            {
                return await _httpClient.GetAsync($"/api/Order/{orderId}");
            }, "GetOrderAsync");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<OrderResponse>>(content);

            _logger.LogDebug("Order fetched successfully: OrderId={OrderId}", apiResponse.Body.Id);

            return apiResponse.Body;
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, string status)
        {
            _logger.LogInformation("Updating order status: OrderId={OrderId}, Status={Status}", orderId, status);

            var request = new { Status = status };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response = await ExecuteWithRetryAsync(async () =>
            {
                return await _httpClient.PutAsync($"/api/Order/{orderId}/status", content);
            }, "UpdateOrderStatusAsync");

            response.EnsureSuccessStatusCode();

            _logger.LogDebug("Order status updated: OrderId={OrderId}, Status={Status}", orderId, status);
        }
    }
}