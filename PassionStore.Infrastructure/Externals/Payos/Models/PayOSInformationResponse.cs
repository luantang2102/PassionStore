
namespace PassionStore.Infrastructure.Externals.Payos.Models
{
	public class PayOSInformationResponse
	{
		public string Id { get; set; } = string.Empty;
		public int OrderCode { get; set; }
		public int Amount { get; set; }
		public int AmountPaid { get; set; }
		public int AmountRemaining { get; set; }
		public string Status { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public ICollection<PayOSTransactionResponse> Transactions { get; set; } = new HashSet<PayOSTransactionResponse>();
		public string CancellationReason { get; set; } = string.Empty;
		public DateTime? CanceledAt { get; set; }
	}
}
