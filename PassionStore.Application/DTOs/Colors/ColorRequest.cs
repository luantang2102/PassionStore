using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassionStore.Application.DTOs.Colors
{
    public class ColorRequest
    {
        public string Name { get; set; } = string.Empty;
        public string HexCode { get; set; } = string.Empty;
    }
}
