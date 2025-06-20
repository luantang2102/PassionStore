using PassionStore.Core.Models.Auth;

namespace PassionStore.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        string GenerateRefreshToken(Guid userId);
        string GenerateToken(AppUser user, IList<string> roles);
        Guid GetIdFromRefreshToken(string refreshToken);
        bool ValidateRefreshToken(string refreshToken);
    }
}
