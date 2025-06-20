using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.Colors;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;

namespace PassionStore.Api.Controllers
{
    public class ColorsController : BaseApiController
    {
        private readonly IColorService _colorService;

        public ColorsController(IColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetColors([FromQuery] ColorParams colorParams)
        {
            var colors = await _colorService.GetColorsAsync(colorParams);
            Response.AddPaginationHeader(colors.Metadata);
            return Ok(colors);
        }

        [HttpGet("tree")]
        public async Task<IActionResult> GetColorsTree()
        {
            var colors = await _colorService.GetColorsAsync();
            return Ok(colors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetColorById(Guid id)
        {
            var color = await _colorService.GetColorByIdAsync(id);
            return Ok(color);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateColor([FromBody] ColorRequest colorRequest)
        {
            var createdColor = await _colorService.CreateColorAsync(colorRequest);
            return CreatedAtAction(nameof(GetColorById), new { id = createdColor.Id }, createdColor);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateColor(Guid id, [FromBody] ColorRequest colorRequest)
        {
            var updatedColor = await _colorService.UpdateColorAsync(id, colorRequest);
            return Ok(updatedColor);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteColor(Guid id)
        {
            await _colorService.DeleteColorAsync(id);
            return NoContent();
        }
    }
}