using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.Sizes;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;

namespace PassionStore.Api.Controllers
{
    public class SizesController : BaseApiController
    {
        private readonly ISizeService _sizeService;

        public SizesController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSizes([FromQuery] SizeParams sizeParams)
        {
            var sizes = await _sizeService.GetSizesAsync(sizeParams);
            Response.AddPaginationHeader(sizes.Metadata);
            return Ok(sizes);
        }

        [HttpGet("tree")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSizesTree()
        {
            var sizes = await _sizeService.GetSizesAsync();
            return Ok(sizes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSizeById(Guid id)
        {
            var size = await _sizeService.GetSizeByIdAsync(id);
            return Ok(size);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSize([FromForm] SizeRequest sizeRequest)
        {
            var createdSize = await _sizeService.CreateSizeAsync(sizeRequest);
            return CreatedAtAction(nameof(GetSizeById), new { id = createdSize.Id }, createdSize);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSize(Guid id, [FromForm] SizeRequest sizeRequest)
        {
            var updatedSize = await _sizeService.UpdateSizeAsync(id, sizeRequest);
            return Ok(updatedSize);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSize(Guid id)
        {
            await _sizeService.DeleteSizeAsync(id);
            return NoContent();
        }
    }
}