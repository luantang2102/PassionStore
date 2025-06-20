
using AssetManagement.Application.Paginations;

namespace PassionStore.Application.Helpers.Params
{
    public class ProductParams : PaginationParams
    {
        public string? OrderBy { get; set; }
        public string? SearchTerm { get; set; }
        public string? Categories { get; set; }
        public string? Colors { get; set; }
        public string? Sizes { get; set; }
        public string? Brands { get; set; }
        public string? Ratings { get; set; }
        public string? MinPrice { get; set; }
        public string? MaxPrice { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? IsSale { get; set; }
        public bool? IsNew { get; set; }

    }
}
