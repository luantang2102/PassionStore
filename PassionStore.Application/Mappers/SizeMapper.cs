using PassionStore.Application.DTOs.Sizes;
using PassionStore.Core.Models;

namespace PassionStore.Application.Mappers
{
    public static class SizeMapper
    {
        public static SizeResponse MapModelToResponse(this Size size)
        {
            return new SizeResponse
            {
                Id = size.Id,
                Name = size.Name,
                CreatedDate = size.CreatedDate,
                UpdatedDate = size.UpdatedDate
            };
        }
    }
}
