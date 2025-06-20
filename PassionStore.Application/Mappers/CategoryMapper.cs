using PassionStore.Application.DTOs.Categories;
using PassionStore.Core.Models;

namespace PassionStore.Application.Mappers
{
    public static class CategoryMapper
    {
        public static CategoryResponse MapModelToResponse(this Category category)
        {
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Level = category.Level,
                IsActive = category.IsActive,
                CreatedDate = category.CreatedDate,
                UpdatedDate = category.UpdatedDate,
                ParentCategoryId = category.ParentCategory?.Id,
                ParentCategoryName = category.ParentCategory?.Name,
                SubCategories = category.SubCategories.Select(c => c.MapModelToResponse()).ToList()
            };
        }

        public static CategoryResponse MapResponseTree(this Category category)
        {
            var response = category.MapModelToResponse();
            response.ParentCategoryName = category.ParentCategory?.Name;
            response.SubCategories = category.SubCategories?.Select(sc => MapResponseTree(sc)).ToList() ?? [];
            return response;
        }
    }
}
