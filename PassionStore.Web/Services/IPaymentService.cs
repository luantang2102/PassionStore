namespace PassionStore.Web.Services
{
    public interface IPaymentService
    {
        Task<string> CreateOrUpdatePaymentIntentAsync();
    }
}