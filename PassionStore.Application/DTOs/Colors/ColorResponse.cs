using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassionStore.Application.DTOs.Colors
{
    public class ColorResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string HexCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
