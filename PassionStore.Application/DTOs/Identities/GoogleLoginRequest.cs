using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassionStore.Application.DTOs.Identities
{
    public class GoogleLoginRequest
    {
        public required string AccessToken { get; set; }
    }
}
