using AssetManagement.Application.Paginations;

namespace PassionStore.Application.Helpers.Params
{
    public class NotificationParams : PaginationParams
    {
        public bool? IsRead { get; set; }
        public string OrderBy { get; set; } = "createdDate_desc";
    }
}