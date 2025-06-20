using System;
using System.Threading.Tasks;
using PassionStore.Application.DTOs.Identities;
using PassionStore.Application.DTOs.Users;

namespace PassionStore.Application.Interfaces.Auth
{
    public interface IIdentityService
    {
        Task<UserResponse> GetCurrentUserAsync(Guid userId);
        Task<UserResponse> RegisterAsync(RegisterRequest registerRequest);
        Task<TokenResponse> LoginAsync(LoginRequest loginRequest);
        Task<TokenResponse> RefreshTokenAsync(string? refreshToken);
        Task ChangePassword(Guid userId, ChangePasswordRequest changePasswordRequest);
        Task SendVerificationCodeAsync(string email);
        Task<TokenResponse> VerifyEmailAsync(string email, string code);
        Task<TokenResponse> GoogleLoginAsync(GoogleLoginRequest googleLoginRequest);
    }
}