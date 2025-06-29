using AssetManagement.Application.Paginations;

namespace PassionStore.Application.Helpers.Params
{
    public class RatingParams : PaginationParams
    {
        public string? OrderBy { get; set; }
        public List<int>? Values { get; set; }
        public string? HasComment { get; set; }
        public string? SearchTerm { get; set; }
    }
}