using AssetManagement.Application.Paginations;

namespace PassionStore.Application.Helpers.Params
{
    public class ProductVariantParams : PaginationParams
    {
        public string? OrderBy { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ColorId { get; set; }
        public Guid? SizeId { get; set; }
        public string? MinPrice { get; set; }
        public string? MaxPrice { get; set; }
    }
}