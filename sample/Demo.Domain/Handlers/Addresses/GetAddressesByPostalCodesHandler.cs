using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts;
using Demo.Api.Generated.Contracts.Addresses;

namespace Demo.Domain.Handlers.Addresses
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Get addresses by postal code.
    /// Operation: GetAddressesByPostalCodes.
    /// Area: Addresses.
    /// </summary>
    public class GetAddressesByPostalCodesHandler : IGetAddressesByPostalCodesHandler
    {
        public Task<GetAddressesByPostalCodesResult> ExecuteAsync(GetAddressesByPostalCodesParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

        public async Task<GetAddressesByPostalCodesResult> InvokeExecuteAsync(GetAddressesByPostalCodesParameters parameters, CancellationToken cancellationToken = default)
        {
            var data = new List<Address>()
            {
                new Address
                {
                    CityName = "Glostrup",
                    PostalCode = "2600",
                    MyCountry = new Country
                    {
                        Name = "Denmark",
                        Alpha2Code = "DK",
                        Alpha3Code = "DNK",
                    },
                },
            };

            return await Task.FromResult(data);
        }
    }
}