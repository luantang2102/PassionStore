using AssetManagement.Application.Paginations;

namespace PassionStore.Application.Helpers.Params
{
    public class UserProfileParams : PaginationParams
    {
        public string? SearchTerm { get; set; }
        public string? OrderBy { get; set; }
    }
}
