﻿using PassionStore.Application.DTOs.Carts;
using PassionStore.Application.DTOs.Ratings;
using PassionStore.Application.DTOs.UserProfiles;

namespace PassionStore.Application.DTOs.Users
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = string.Empty;
        public string? PublicId { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public required bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; } = [];
        public List<UserProfileResponse> UserProfiles { get; set; } = [];
        public CartResponse Cart { get; set; } = new CartResponse();
        public List<RatingResponse> Ratings { get; set; } = [];

    }
}
