using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Interfaces.IServices;

namespace PassionStore.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ICartRepository _cartRepository;

        public PaymentService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public Task<string?> CreateOrUpdatePaymentIntentAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}