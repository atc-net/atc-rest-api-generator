using System;
using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Orders;

namespace Demo.Domain.Handlers.Orders
{
    public class PatchOrdersIdHandler : IPatchOrdersIdHandler
    {
        public Task<PatchOrdersIdResult> ExecuteAsync(PatchOrdersIdParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return parameters.Id.ToString() switch
            {
                "77a33260-0008-441f-ba60-b0a833803fab" => throw new Exception("Database broken!"),
                "77a33260-0009-441f-ba60-b0a833803fab" => Task.FromResult(PatchOrdersIdResult.Conflict()),
                "77a33260-0010-441f-ba60-b0a833803fab" => Task.FromResult(PatchOrdersIdResult.Conflict("Something is broken - maybe a horse!")),
                "77a33260-0011-441f-ba60-b0a833803fab" => Task.FromResult(PatchOrdersIdResult.BadGateway("Something is broken - maybe a horse!")),
                _ => InvokeExecuteAsync(parameters, cancellationToken)
            };
        }

        private async Task<PatchOrdersIdResult> InvokeExecuteAsync(PatchOrdersIdParameters parameters, CancellationToken cancellationToken)
        {
            return await Task.FromResult("Data is updated");
        }
    }
}