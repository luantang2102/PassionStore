using AssetManagement.Application.Paginations;

namespace PassionStore.Application.Helpers.Params
{
    public class MessageParams : PaginationParams
    {
        public string SearchTerm { get; set; } = string.Empty;
        public string OrderBy { get; set; } = "CreatedDate";
        public bool? IsUserMessage { get; set; }
    }
}