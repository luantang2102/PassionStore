using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.Categories;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;

namespace PassionStore.Api.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryService _categoryService;

        public ICategoryService CategoryService => _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] CategoryParams categoryParams)
        {
            var categories = await CategoryService.GetCategoriesAsync(categoryParams);
            Response.AddPaginationHeader(categories.Metadata);
            return Ok(categories);
        }

        [HttpGet("tree")]
        public async Task<IActionResult> GetCategoriesTree()
        {
            var categories = await CategoryService.GetCategoriesTreeAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await CategoryService.GetCategoryByIdAsync(id);
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryRequest categoryRequest)
        {
            var createdCategory = await CategoryService.CreateCategoryAsync(categoryRequest);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromForm] CategoryRequest categoryRequest)
        {
            var updatedCategory = await CategoryService.UpdateCategoryAsync(id, categoryRequest);
            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            await CategoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
