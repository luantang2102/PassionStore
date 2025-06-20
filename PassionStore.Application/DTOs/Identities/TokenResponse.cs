using PassionStore.Application.DTOs.Users;

namespace PassionStore.Application.DTOs.Identities
{
    public class TokenResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required UserResponse UserResponse { get; set; }
    }
}
