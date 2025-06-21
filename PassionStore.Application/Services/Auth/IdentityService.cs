using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using PassionStore.Application.DTOs.Identities;
using PassionStore.Application.DTOs.Users;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Interfaces.Auth;
using PassionStore.Application.Mappers;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Core.Models.Auth;
using PassionStore.Infrastructure.Externals;

namespace PassionStore.Application.Services.Auth
{
    public class IdentityService : IIdentityService
    {
        private readonly ITokenService _jwt;
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly EmailService _emailService;
        private readonly IVerifyCodeRepository _verifyCodeRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly string _verificationEmailTemplate;

        public IdentityService(
            ITokenService jwt,
            IUserRepository userRepository,
            ICartRepository cartRepository,
            IUnitOfWork unitOfWork,
            EmailService emailService,
            IVerifyCodeRepository verifyCodeRepository,
            IMemoryCache memoryCache)
        {
            _jwt = jwt;
            _userRepository = userRepository;
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _verifyCodeRepository = verifyCodeRepository;
            _memoryCache = memoryCache;

            // Load the email template at startup
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "VerificationEmail.html");
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException("Verification email template not found.", templatePath);
            }
            _verificationEmailTemplate = File.ReadAllText(templatePath);
        }

        public async Task<UserResponse> GetCurrentUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }
            var roles = await _userRepository.GetRolesAsync(user);
            return user.MapModelToResponse(roles);
        }

        public async Task<TokenResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userRepository.FindByEmailAsync(loginRequest.Email);
            if (user == null || !await _userRepository.CheckPasswordAsync(user, loginRequest.Password))
            {
                var attributes = new Dictionary<string, object>
                {
                    { "email", loginRequest.Email }
                };
                throw new AppException(ErrorCode.INVALID_CREDENTIALS, attributes);
            }

            if (!user.EmailConfirmed)
            {
                // Invalidate existing verification codes
                await _verifyCodeRepository.DeleteAllByUserIdAsync(user.Id);

                // Generate and send new verification code
                var verificationCode = GenerateVerificationCode();
                var verifyCode = new VerifyCode
                {
                    Code = verificationCode,
                    UserId = user.Id,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                    IsVerified = false
                };
                await _verifyCodeRepository.CreateAsync(verifyCode);
                await _unitOfWork.CommitAsync();

                var emailBody = GetVerificationEmailHtml(verificationCode);
                await _emailService.SendEmailAsync(user.Email!, "Verify Your Email", plainText: null, htmlContent: emailBody);

                throw new AppException(ErrorCode.EMAIL_NOT_VERIFIED, new Dictionary<string, object>
                {
                    { "message", "Email not verified, new code sent" }
                });
            }

            var cart = await _cartRepository.GetByUserIdAsync(user.Id);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = user.Id,
                    CartItems = []
                };
                try
                {
                    await _cartRepository.CreateAsync(cart);
                }
                catch (Exception ex)
                {
                    throw new AppException(ErrorCode.CART_CREATION_FAILED);
                }
            }
            
            var userInfo = await _userRepository.GetByIdAsync(user.Id);
            if (userInfo == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }
            var roles = await _userRepository.GetRolesAsync(user);
            var accessToken = _jwt.GenerateToken(user, roles);
            var refreshToken = _jwt.GenerateRefreshToken(user.Id);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserResponse = userInfo.MapModelToResponse(roles),
            };
        }

        public async Task<UserResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                throw new AppException(ErrorCode.PASSWORDS_DO_NOT_MATCH);
            }

            var existingUser = await _userRepository.FindByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                throw new AppException(ErrorCode.USER_ALREADY_EXISTS);
            }

            var user = registerRequest.MapToModel();
            user.EmailConfirmed = false;

            var result = await _userRepository.CreateAsync(user, registerRequest.Password);
            if (!result)
            {
                throw new AppException(ErrorCode.IDENTITY_CREATION_FAILED);
            }

            await _userRepository.AddToRoleAsync(user, UserRole.User.ToString());

            var cart = new Cart
            {
                UserId = user.Id,
                CartItems = []
            };
            await _cartRepository.CreateAsync(cart);

            // Invalidate existing verification codes
            await _verifyCodeRepository.DeleteAllByUserIdAsync(user.Id);

            // Generate and send verification code
            var verificationCode = GenerateVerificationCode();
            var verifyCode = new VerifyCode
            {
                Code = verificationCode,
                UserId = user.Id,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                IsVerified = false
            };
            await _verifyCodeRepository.CreateAsync(verifyCode);
            await _unitOfWork.CommitAsync();

            var emailBody = GetVerificationEmailHtml(verificationCode);
            await _emailService.SendEmailAsync(user.Email!, "Verify Your Email", plainText: null, htmlContent: emailBody);


            return user.MapModelToResponse();
        }

        public async Task<TokenResponse> RefreshTokenAsync(string? refreshToken)
        {
            if (refreshToken == null)
            {
                throw new AppException(ErrorCode.REFRESH_TOKEN_NOT_FOUND);
            }

            if (!_jwt.ValidateRefreshToken(refreshToken))
            {
                throw new AppException(ErrorCode.INVALID_REFRESH_TOKEN);
            }

            var userId = _jwt.GetIdFromRefreshToken(refreshToken);
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(ErrorCode.ACCOUNT_NOT_FOUND);
            }

            var roles = await _userRepository.GetRolesAsync(user);
            var newAccessToken = _jwt.GenerateToken(user, roles);
            var newRefreshToken = _jwt.GenerateRefreshToken(userId);

            return new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserResponse = user.MapModelToResponse(roles),
            };
        }

        public async Task ChangePassword(Guid userId, ChangePasswordRequest changePasswordRequest)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }
            if (!await _userRepository.CheckPasswordAsync(user, changePasswordRequest.OldPassword))
            {
                throw new AppException(ErrorCode.INVALID_OLD_PASSWORD);
            }

            var result = await _userRepository.ChangePasswordAsync(user, changePasswordRequest.NewPassword);
            if (!result)
            {
                throw new AppException(ErrorCode.SAVE_ERROR);
            }

            await _unitOfWork.CommitAsync();
        }

        public async Task SendVerificationCodeAsync(string email)
        {
            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }

            if (user.EmailConfirmed)
            {
                throw new AppException(ErrorCode.EMAIL_ALREADY_VERIFIED);
            }

            // Rate limiting: 3 requests per hour per email
            var cacheKey = $"VerificationCode:{email}";
            if (_memoryCache.TryGetValue(cacheKey, out int requestCount) && requestCount >= 3)
            {
                throw new AppException(ErrorCode.TOO_MANY_REQUESTS);
            }

            // Invalidate existing verification codes
            await _verifyCodeRepository.DeleteAllByUserIdAsync(user.Id);

            // Generate and send new verification code
            var verificationCode = GenerateVerificationCode();
            var verifyCode = new VerifyCode
            {
                Code = verificationCode,
                UserId = user.Id,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                IsVerified = false
            };
            await _verifyCodeRepository.CreateAsync(verifyCode);

            var emailBody = GetVerificationEmailHtml(verificationCode);
            await _emailService.SendEmailAsync(email, "Verify Your Email", plainText: null, htmlContent: emailBody);

            await _unitOfWork.CommitAsync();

            // Update rate limit cache
            requestCount = _memoryCache.TryGetValue(cacheKey, out int count) ? count + 1 : 1;
            _memoryCache.Set(cacheKey, requestCount, TimeSpan.FromHours(1));
        }

        public async Task<TokenResponse> VerifyEmailAsync(string email, string code)
        {
            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }

            if (user.EmailConfirmed)
            {
                throw new AppException(ErrorCode.EMAIL_ALREADY_VERIFIED);
            }

            var verifyCode = await _verifyCodeRepository.GetByUserIdAndCodeAsync(user.Id, code);
            if (verifyCode == null)
            {
                throw new AppException(ErrorCode.INVALID_VERIFICATION_CODE);
            }

            if (verifyCode.IsVerified)
            {
                throw new AppException(ErrorCode.VERIFICATION_CODE_ALREADY_USED);
            }

            if (verifyCode.ExpiryTime < DateTime.UtcNow)
            {
                throw new AppException(ErrorCode.VERIFICATION_CODE_EXPIRED);
            }

            verifyCode.IsVerified = true;
            user.EmailConfirmed = true;

            await _userRepository.UpdateAsync(user);
            await _verifyCodeRepository.DeleteAsync(verifyCode); // Delete the used code
            await _unitOfWork.CommitAsync();

            var userInfo = await _userRepository.GetByIdAsync(user.Id);
            if (userInfo == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }
            var roles = await _userRepository.GetRolesAsync(user);
            var accessToken = _jwt.GenerateToken(user, roles);
            var refreshToken = _jwt.GenerateRefreshToken(user.Id);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserResponse = user.MapModelToResponse(roles),
            };
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit code
        }

        private string GetVerificationEmailHtml(string code)
        {
            return _verificationEmailTemplate.Replace("{{code}}", code);
        }

        public async Task<TokenResponse> GoogleLoginAsync(GoogleLoginRequest googleLoginRequest)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={googleLoginRequest.AccessToken}");
            if (!response.IsSuccessStatusCode)
            {
                throw new AppException(ErrorCode.GOOGLE_LOGIN_FAILED);
            }
            var content = await response.Content.ReadAsStringAsync();

            var googleUser = JsonConvert.DeserializeObject<GoogleLoginResponse>(content);
            if(googleUser == null || string.IsNullOrEmpty(googleUser.Email))
            {
                throw new AppException(ErrorCode.GOOGLE_LOGIN_FAILED);
            }

            var user = await _userRepository.FindByEmailAsync(googleUser.Email);
            if (user == null)
            {
                user = googleUser.MapToModel();
                var result = await _userRepository.CreateAsync(user, null);
                if (!result)
                {
                    throw new AppException(ErrorCode.IDENTITY_CREATION_FAILED);
                }
                await _userRepository.AddToRoleAsync(user, UserRole.User.ToString());
                var cart = new Cart
                {
                    UserId = user.Id,
                    CartItems = []
                };
                await _cartRepository.CreateAsync(cart);
            }
            else
            {
                user.ImageUrl = googleUser.Picture;
                await _userRepository.UpdateAsync(user);
            }

            var userInfo = await _userRepository.GetByIdAsync(user.Id);
            if (userInfo == null)
            {
                throw new AppException(ErrorCode.USER_NOT_FOUND);
            }

            var roles = await _userRepository.GetRolesAsync(user);
            var accessToken = _jwt.GenerateToken(user, roles);
            var refreshToken = _jwt.GenerateRefreshToken(user.Id);

            var cartCheck = await _cartRepository.GetByUserIdAsync(userInfo.Id);
            if (cartCheck == null)
            {
                cartCheck = new Cart
                {
                    UserId = user.Id,
                    CartItems = []
                };
                try
                {
                    await _cartRepository.CreateAsync(cartCheck);
                }
                catch (Exception ex)
                {
                    throw new AppException(ErrorCode.CART_CREATION_FAILED);
                }
            }

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserResponse = userInfo.MapModelToResponse(roles),
            };

        }
    }
}