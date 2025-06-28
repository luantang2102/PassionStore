using Newtonsoft.Json;

namespace PassionStore.Infrastructure.Externals.Payos.Models
{
    public class PayOSResponse<T>
    {
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty; // Changed from int to string

        [JsonProperty("desc")]
        public string Desc { get; set; } = string.Empty;

        [JsonProperty("data")]
        public T? Data { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; } = string.Empty;
    }
}