using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.Identities;
using PassionStore.Application.Interfaces.Auth;

namespace PassionStore.Api.Controllers.Auth
{
    public class AuthController : BaseApiController
    {
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
        {
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
        }

        private void SetAuthCookies(string accessToken, string refreshToken)
        {
            var jwtCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddHours(1),
                Path = "/"
            };
            _httpContextAccessor.HttpContext!.Response.Cookies.Append("auth_jwt", accessToken, jwtCookieOptions);

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddDays(7),
                Path = "/"
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append("refresh", refreshToken, refreshCookieOptions);
        }

        private void ClearAuthCookies()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            };
            _httpContextAccessor.HttpContext!.Response.Cookies.Delete("auth_jwt", cookieOptions);
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("refresh", cookieOptions);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest loginRequest)
        {
            var tokenResponse = await _identityService.LoginAsync(loginRequest);
            SetAuthCookies(tokenResponse.AccessToken, tokenResponse.RefreshToken);
            return Ok(tokenResponse.UserResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest registerRequest)
        {
            var response = await _identityService.RegisterAsync(registerRequest);
            return CreatedAtAction(nameof(Register), new { id = response.Id }, response);
        }

        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh"];
            var tokenResponse = await _identityService.RefreshTokenAsync(refreshToken);
            SetAuthCookies(tokenResponse.AccessToken, tokenResponse.RefreshToken);
            return Ok(tokenResponse.UserResponse);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            ClearAuthCookies();
            return NoContent();
        }

        [HttpGet("check")]
        [Authorize]
        public async Task<IActionResult> CheckAuth()
        {
            var userId = User.GetUserId();
            var userResponse = await _identityService.GetCurrentUserAsync(userId);
            return Ok(userResponse);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordRequest changePasswordRequest)
        {
            var userId = User.GetUserId();
            await _identityService.ChangePassword(userId, changePasswordRequest);
            return NoContent();
        }

        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] EmailRequest emailRequest)
        {
            await _identityService.SendVerificationCodeAsync(emailRequest.Email);
            return NoContent();
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest verifyEmailRequest)
        {
            var tokenResponse = await _identityService.VerifyEmailAsync(verifyEmailRequest.Email, verifyEmailRequest.Code);
            SetAuthCookies(tokenResponse.AccessToken, tokenResponse.RefreshToken);
            return Ok(tokenResponse.UserResponse);
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest googleLoginRequest)
        {
            var tokenResponse = await _identityService.GoogleLoginAsync(googleLoginRequest);
            SetAuthCookies(tokenResponse.AccessToken, tokenResponse.RefreshToken);
            return Ok(tokenResponse.UserResponse);
        }
    }
}