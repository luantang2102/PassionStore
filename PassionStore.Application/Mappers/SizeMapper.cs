using PassionStore.Application.DTOs.Sizes;
using PassionStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
