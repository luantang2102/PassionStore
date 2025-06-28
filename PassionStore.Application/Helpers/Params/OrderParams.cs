using AssetManagement.Application.Paginations;

namespace PassionStore.Application.Helpers.Params
{
    public class OrderParams : PaginationParams
    {
        public string? SearchTerm { get; set; }
        public string? OrderBy { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }

    }
}