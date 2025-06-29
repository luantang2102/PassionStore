using AssetManagement.Application.Paginations;
using System.Collections.Generic;

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