using PassionStore.Application.DTOs.Products;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;
namespace PassionStore.Application.Interfaces
{
    public interface IProductService
    {
        Task<PagedList<ProductResponse>> GetProductsAsync(ProductParams productParams);
        Task<ProductResponse> GetProductByIdAsync(Guid productId);
        Task<PagedList<ProductResponse>> GetProductsByCategoryIdAsync(Guid categoryId, ProductParams productParams);
        Task<ProductResponse> CreateProductAsync(ProductRequest productRequest);
        Task<ProductResponse> UpdateProductAsync(Guid productId, ProductRequest productRequest);
        Task DeleteProductAsync(Guid productId);

    }
}
