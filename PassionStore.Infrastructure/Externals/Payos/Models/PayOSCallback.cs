namespace PassionStore.Infrastructure.Externals.Payos.Models
{
    public class PayOSCallback
    {
        public string Code { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public bool Cancel { get; set; }
        public string Status { get; set; } = string.Empty;
        public int OrderCode { get; set; }
    }
}
