using PassionStore.Application.DTOs.Products;
using PassionStore.Application.DTOs.ProductVariants;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;
using PassionStore.Core.Models;

namespace PassionStore.Application.Interfaces
{
    public interface IProductVariantService
    {
        Task<ProductVariantResponse> CreateAsync(ProductVariantRequest request);
        Task DeleteAsync(Guid productVariantId);
        Task<ProductVariantResponse> GetProductVariantByIdAsync(Guid productVariantId);
        Task<PagedList<ProductVariantResponse>> GetProductVariantsAsync(ProductVariantParams productVariantParams);
        Task<ProductVariantResponse> UpdateAsync(Guid productVariantId, ProductVariantRequest request);
    }
}