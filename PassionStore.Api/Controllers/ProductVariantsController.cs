using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.ProductVariants;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;

namespace PassionStore.Api.Controllers
{
    public class ProductVariantsController : BaseApiController
    {
        private readonly IProductVariantService _productVariantService;

        public ProductVariantsController(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductVariants([FromQuery] ProductVariantParams productVariantParams)
        {
            var variants = await _productVariantService.GetProductVariantsAsync(productVariantParams);
            Response.AddPaginationHeader(variants.Metadata);
            return Ok(variants);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductVariantById(Guid id)
        {
            var variant = await _productVariantService.GetProductVariantByIdAsync(id);
            return Ok(variant);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProductVariant([FromForm] ProductVariantRequest productVariantRequest)
        {
            var createdVariant = await _productVariantService.CreateProductVariantAsync(productVariantRequest);
            return CreatedAtAction(nameof(GetProductVariantById), new { id = createdVariant.Id }, createdVariant);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductVariant(Guid id, [FromForm] ProductVariantRequest productVariantRequest)
        {
            var updatedVariant = await _productVariantService.UpdateProductVariantAsync(id, productVariantRequest);
            return Ok(updatedVariant);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductVariant(Guid id)
        {
            await _productVariantService.DeleteProductVariantAsync(id);
            return NoContent();
        }
    }
}