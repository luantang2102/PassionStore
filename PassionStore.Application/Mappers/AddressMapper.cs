using PassionStore.Application.DTOs.Addresses;
using PassionStore.Core.Models;

namespace PassionStore.Application.Mappers
{
    public static class AddressMapper
    {
        public static AddressResponse MapModelToResponse(this Address address)
        {
            return new AddressResponse
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country
            };
        }
    }
}
