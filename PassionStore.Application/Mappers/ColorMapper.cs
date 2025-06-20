using PassionStore.Application.DTOs.Colors;
using PassionStore.Core.Models;


namespace PassionStore.Application.Mappers
{
    public static class ColorMapper
    {
        public static ColorResponse MapModelToResponse(this Color color)
        {
            return new ColorResponse
            {
                Id = color.Id,
                Name = color.Name,
                HexCode = color.HexCode,
                CreatedDate = color.CreatedDate,
                UpdatedDate = color.UpdatedDate
            };
        }
    }
}
