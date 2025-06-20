using PassionStore.Application.DTOs.Sizes;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;

namespace PassionStore.Application.Interfaces
{
    public interface ISizeService
    {
        Task<SizeResponse> GetSizeByIdAsync(Guid sizeId);
        Task<PagedList<SizeResponse>> GetSizesAsync(SizeParams sizeParams);
        Task<SizeResponse> CreateSizeAsync(SizeRequest sizeRequest);
        Task<SizeResponse> UpdateSizeAsync(Guid sizeId, SizeRequest sizeRequest);
        Task DeleteSizeAsync(Guid sizeId);
        Task<List<SizeResponse>> GetSizesAsync();
    }
}