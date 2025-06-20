using PassionStore.Application.DTOs.Colors;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Paginations;

namespace PassionStore.Application.Interfaces
{
    public interface IColorService
    {
        Task<ColorResponse> GetColorByIdAsync(Guid colorId);
        Task<PagedList<ColorResponse>> GetColorsAsync(ColorParams colorParams);
        Task<ColorResponse> CreateColorAsync(ColorRequest colorRequest);
        Task<ColorResponse> UpdateColorAsync(Guid colorId, ColorRequest colorColorRequest);
        Task DeleteColorAsync(Guid colorId);
        Task<List<ColorResponse>> GetColorsAsync();
    }
}