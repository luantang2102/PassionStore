﻿using Microsoft.EntityFrameworkCore;
using PassionStore.Application.DTOs.Categories;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Mappers;
using PassionStore.Application.Paginations;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Extensions;


namespace PassionStore.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CategoryRequest categoryRequest)
        {
            int level = 0;

            if (categoryRequest.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(categoryRequest.ParentCategoryId.Value);
                if (parentCategory == null)
                {
                    var attributes = new Dictionary<string, object>
                        {
                            { "parentCategoryId", categoryRequest.ParentCategoryId.Value }
                        };
                    throw new AppException(ErrorCode.PARENT_CATEGORY_NOT_FOUND, attributes);
                }

                level = parentCategory.Level + 1;
            }

            var category = new Category
            {
                Name = categoryRequest.Name,
                Description = categoryRequest.Description,
                IsActive = categoryRequest.IsActive,
                ParentCategoryId = categoryRequest.ParentCategoryId,
                Level = level
            };

            var createdCategory = await _categoryRepository.CreateAsync(category);

            await _unitOfWork.CommitAsync();

            return createdCategory.MapModelToResponse();
        }

        public async Task<bool> DeleteCategoryAsync(Guid categoryId)
        {
            var category = await _categoryRepository.GetWithSubCategoriesAsync(categoryId);
            if (category == null)
            {
                var attributes = new Dictionary<string, object>
                    {
                        { "categoryId", categoryId }
                    };
                throw new AppException(ErrorCode.CATEGORY_NOT_FOUND, attributes);
            }

            if (category.SubCategories.Any())
            {
                var attributes = new Dictionary<string, object>
                    {
                        { "categoryId", categoryId },
                        { "subCategoriesCount", category.SubCategories.Count }
                    };
                throw new AppException(ErrorCode.CATEGORY_HAS_SUBCATEGORIES, attributes);
            }

            if (await _categoryRepository.HasProduct(categoryId))
            {
                var attributes = new Dictionary<string, object>
                    {
                        { "categoryId", categoryId }
                    };
                throw new AppException(ErrorCode.CATEGORY_HAS_PRODUCTS, attributes);
            }

            await _categoryRepository.DeleteAsync(category);
            return true;
        }

        public async Task<PagedList<CategoryResponse>> GetCategoriesAsync(CategoryParams categoryParams)
        {
            var query = _categoryRepository.GetAllAsync();

            if (categoryParams.ParentCategoryId.HasValue)
            {
                var parentExists = await _categoryRepository.ParentCategoryExistsAsync(categoryParams.ParentCategoryId.Value);
                if (!parentExists)
                {
                    var attributes = new Dictionary<string, object>
                        {
                            { "parentCategoryId", categoryParams.ParentCategoryId.Value }
                        };
                    throw new AppException(ErrorCode.PARENT_CATEGORY_NOT_FOUND, attributes);
                }
                query = query.Where(c => c.ParentCategoryId == categoryParams.ParentCategoryId);
            }

            query = query
                .Sort(categoryParams.OrderBy)
                .Search(categoryParams.SearchTerm)
                .Filter(categoryParams.Level, categoryParams.IsActive);

            var projectedQuery = query.Select(x => x.MapModelToResponse());

            return await PaginationService.ToPagedList(
                projectedQuery,
                categoryParams.PageNumber,
                categoryParams.PageSize
            );
        }

        public async Task<List<CategoryResponse>> GetCategoriesByIdsAsync(List<Guid> categoryIds)
        {
            var categories = await _categoryRepository.GetByIdsAsync(categoryIds);
            if (categories.Count == 0)
            {
                var attributes = new Dictionary<string, object>
                    {
                        { "categoryIds", string.Join(", ", categoryIds) }
                    };
                throw new AppException(ErrorCode.CATEGORY_NOT_FOUND, attributes);
            }

            return categories.Select(c => c.MapModelToResponse()).ToList();
        }

        public async Task<List<CategoryResponse>> GetCategoriesTreeAsync()
        {
            var rootCategories = await _categoryRepository
                .GetRootCategoriesAsync()
                .Where(x => x.IsActive)
                .ToListAsync();
            return rootCategories.Select(c => c.MapResponseTree()).ToList();
        }



        public async Task<CategoryResponse> GetCategoryByIdAsync(Guid categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                var attributes = new Dictionary<string, object>
                    {
                        { "categoryId", categoryId }
                    };
                throw new AppException(ErrorCode.CATEGORY_NOT_FOUND, attributes);
            }

            return category.MapModelToResponse();
        }

        public async Task<CategoryResponse> UpdateCategoryAsync(Guid categoryId, CategoryRequest categoryRequest)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                var attributes = new Dictionary<string, object>
                    {
                        { "categoryId", categoryId }
                    };
                throw new AppException(ErrorCode.CATEGORY_NOT_FOUND, attributes);
            }

            if (categoryRequest.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(categoryRequest.ParentCategoryId.Value);
                if (parentCategory == null)
                {
                    var attributes = new Dictionary<string, object>
                        {
                            { "parentCategoryId", categoryRequest.ParentCategoryId.Value }
                        };
                    throw new AppException(ErrorCode.PARENT_CATEGORY_NOT_FOUND, attributes);
                }

                category.Level = parentCategory.Level + 1;
            }
            else
            {
                category.Level = 0;
            }

            if (!categoryRequest.IsActive)
            {
                await DeactiveCategoriesBranch(categoryId);
            }
            else
            {
                category.IsActive = categoryRequest.IsActive;
            }

            category.Name = categoryRequest.Name;
            category.Description = categoryRequest.Description;
            category.ParentCategoryId = categoryRequest.ParentCategoryId;
            category.UpdatedDate = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);
            return category.MapModelToResponse();
        }

        private async Task DeactiveCategoriesBranch(Guid parentCategoryId)
        {
            var parentCategory = await _categoryRepository.GetWithSubCategoriesAsync(parentCategoryId);
            if (parentCategory == null) return;

            // Recursively deactivate the branch
            await DeactiveCategoryAndSubcategories(parentCategory);
        }

        private async Task DeactiveCategoryAndSubcategories(Category category)
        {
            category.IsActive = false;
            category.UpdatedDate = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);

            if (category.SubCategories != null && category.SubCategories.Any())
            {
                foreach (var subCategory in category.SubCategories)
                {
                    // Fetch the subcategory with its own subcategories
                    var subCategoryWithChildren = await _categoryRepository.GetWithSubCategoriesAsync(subCategory.Id);
                    if (subCategoryWithChildren != null)
                    {
                        await DeactiveCategoryAndSubcategories(subCategoryWithChildren);
                    }
                }
            }
        }
    }
}