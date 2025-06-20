using PassionStore.Application.DTOs.Categories;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;

namespace PassionStore.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedList<CategoryResponse>> GetCategoriesAsync(CategoryParams categoryParams);
        Task<CategoryResponse> GetCategoryByIdAsync(Guid categoryId);
        Task<List<CategoryResponse>> GetCategoriesByIdsAsync(List<Guid> categoryIds);
        Task<CategoryResponse> CreateCategoryAsync(CategoryRequest categoryRequest);
        Task<CategoryResponse> UpdateCategoryAsync(Guid categoryId, CategoryRequest categoryRequest);
        Task<bool> DeleteCategoryAsync(Guid categoryId);
        Task<List<CategoryResponse>> GetCategoriesTreeAsync();
    }
}
