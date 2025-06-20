using PassionStore.Application.DTOs.Products;
using PassionStore.Application.DTOs.ProductVariants;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;

namespace PassionStore.Application.Interfaces
{
    public interface IProductVariantService
    {
        Task<ProductVariantResponse> GetProductVariantByIdAsync(Guid productVariantId);
        Task<PagedList<ProductVariantResponse>> GetProductVariantsAsync(ProductVariantParams productVariantParams);
        Task<ProductVariantResponse> CreateProductVariantAsync(ProductVariantRequest productVariantRequest);
        Task<ProductVariantResponse> UpdateProductVariantAsync(Guid productVariantId, ProductVariantRequest productVariantRequest);
        Task DeleteProductVariantAsync(Guid productVariantId);
    }
}