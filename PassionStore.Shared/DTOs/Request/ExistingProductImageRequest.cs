﻿using System;

namespace PassionStore.Shared.DTOs.Request
{
    public class ExistingProductImageRequest
    {
        public Guid Id { get; set; }
        public bool IsMain { get; set; }
    }
}
