using PassionStore.Application.DTOs.Brands;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;

namespace PassionStore.Application.Interfaces
{
    public interface IBrandService
    {
        Task<BrandResponse> GetBrandByIdAsync(Guid brandId);
        Task<PagedList<BrandResponse>> GetBrandsAsync(BrandParams brandParams);
        Task<BrandResponse> CreateBrandAsync(BrandRequest brandRequest);
        Task<BrandResponse> UpdateBrandAsync(Guid brandId, BrandRequest brandRequest);
        Task DeleteBrandAsync(Guid brandId);
        Task<List<BrandResponse>> GetBrandsAsync();
    }
}