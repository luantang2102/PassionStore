using Newtonsoft.Json;

namespace PassionStore.Infrastructure.Externals.Payos.Models
{
    public class PayOSDetailResponse
    {
        [JsonProperty("bin")]
        public string Bin { get; set; } = string.Empty;

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; } = string.Empty;

        [JsonProperty("accountName")]
        public string AccountName { get; set; } = string.Empty;

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("orderCode")]
        public int OrderCode { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonProperty("paymentLinkId")]
        public string PaymentLinkId { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("checkoutUrl")]
        public string CheckoutUrl { get; set; } = string.Empty;

        [JsonProperty("qrCode")]
        public string QrCode { get; set; } = string.Empty;
    }
}