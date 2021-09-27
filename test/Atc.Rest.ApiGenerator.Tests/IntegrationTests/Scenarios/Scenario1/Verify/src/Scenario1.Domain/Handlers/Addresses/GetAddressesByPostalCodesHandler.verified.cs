using System.Threading;
using System.Threading.Tasks;
using Scenario1.Api.Generated.Contracts.Addresses;

namespace Scenario1.Domain.Handlers.Addresses
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<GetAddressesByPostalCodesResult> InvokeExecuteAsync(GetAddressesByPostalCodesParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}