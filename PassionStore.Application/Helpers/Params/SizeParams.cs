using AssetManagement.Application.Paginations;

namespace PassionStore.Application.Helpers.Params
{
    public class SizeParams : PaginationParams
    {
        public string? OrderBy { get; set; }
        public string? SearchTerm { get; set; }
    }
}