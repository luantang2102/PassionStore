using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.Brands;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;

namespace PassionStore.Api.Controllers
{
    public class BrandsController : BaseApiController
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands([FromQuery] BrandParams brandParams)
        {
            var brands = await _brandService.GetBrandsAsync(brandParams);
            Response.AddPaginationHeader(brands.Metadata);
            return Ok(brands);
        }

        [HttpGet("tree")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBrandsTree()
        {
            var brands = await _brandService.GetBrandsAsync();
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(Guid id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            return Ok(brand);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBrand([FromForm] BrandRequest brandRequest)
        {
            var createdBrand = await _brandService.CreateBrandAsync(brandRequest);
            return CreatedAtAction(nameof(GetBrandById), new { id = createdBrand.Id }, createdBrand);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBrand(Guid id, [FromForm] BrandRequest brandRequest)
        {
            var updatedBrand = await _brandService.UpdateBrandAsync(id, brandRequest);
            return Ok(updatedBrand);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            await _brandService.DeleteBrandAsync(id);
            return NoContent();
        }
    }
}