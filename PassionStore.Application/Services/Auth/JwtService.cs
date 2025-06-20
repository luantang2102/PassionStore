using Microsoft.IdentityModel.Tokens;
using PassionStore.Application.Interfaces.Auth;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Models.Auth;
using PassionStore.Infrastructure.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AssetManagement.Application.Services.Auth
{
    public class JwtService : ITokenService
    {
        private readonly JwtOption _jwt;

        public JwtService(JwtOption jwtOption)
        {
            _jwt = jwtOption;
        }


        public string GenerateToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        public string GenerateRefreshToken(Guid accountId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, accountId.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateRefreshToken(string refreshToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwt.Key);

                tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwt.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return validatedToken != null;
            }
            catch
            {
                return false;
            }
        }

        public Guid GetIdFromRefreshToken(string refreshToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(refreshToken);
                var accountIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !Guid.TryParse(accountIdClaim.Value, out Guid accountId))
                {
                    throw new AccessDeniedException(ErrorCode.INVALID_REFRESH_TOKEN);
                }
                return accountId;
            }
            catch
            {
                throw new AccessDeniedException(ErrorCode.INVALID_REFRESH_TOKEN);
            }
        }
    }
}