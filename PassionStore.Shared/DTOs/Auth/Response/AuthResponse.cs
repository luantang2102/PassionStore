using PassionStore.Shared.DTOs.Response;

namespace PassionStore.Shared.DTOs.Auth.Response
{
    public class AuthResponse
    {
        public string CsrfToken { get; set; } = string.Empty;
        public UserResponse User { get; set; } = null!;
    }
}
