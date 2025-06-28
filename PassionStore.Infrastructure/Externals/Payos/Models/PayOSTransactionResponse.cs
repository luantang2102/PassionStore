namespace PassionStore.Infrastructure.Externals.Payos.Models
{
	public class PayOSTransactionResponse
	{
		public string Reference { get; set; } = string.Empty;
		public int Amount { get; set; }
		public string AccountNumber { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime TransactionDateTime { get; set; }
		public string VirtualAccountName { get; set; } = string.Empty;
		public string VirtualAccountNumber { get; set; } = string.Empty;
		public string CounterAccountBankId { get; set; } = string.Empty;
		public string CounterAccountBankName { get; set; } = string.Empty;
		public string CounterAccountName { get; set; } = string.Empty;
		public string CounterAccountNumber { get; set; } = string.Empty;
	}
}
