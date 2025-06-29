namespace PassionStore.Application.DTOs.Ratings
{
    public class RatingResponse
    {
        public Guid Id { get; set; }
        public int Value { get; set; }
        public string? Comment { get; set; } = string.Empty;
        public int Helpful { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public Guid ProductId { get; set; } = Guid.Empty;
    }
}
