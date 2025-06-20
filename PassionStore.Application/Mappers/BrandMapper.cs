using PassionStore.Application.DTOs.Brands;
using PassionStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassionStore.Application.Mappers
{
    public static class BrandMapper
    {
        public static BrandResponse MapModelToResponse(this Brand brand)
        {
            return new BrandResponse
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                CreatedDate = brand.CreatedDate,
                UpdatedDate = brand.UpdatedDate
            };
        }
    }
}
