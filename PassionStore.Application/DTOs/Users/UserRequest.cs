using Microsoft.AspNetCore.Http;

namespace PassionStore.Application.DTOs.Users
{
    public class UserRequest
    {
        public IFormFile? Image { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

    }
}
