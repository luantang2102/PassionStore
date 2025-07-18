﻿namespace PassionStore.Infrastructure.Settings
{
    public class PayOSOption
    {
        public string ClientId { get; set; } = string.Empty;
        public string APIKey { get; set; } = string.Empty;
        public string ChecksumKey { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
    }
}
