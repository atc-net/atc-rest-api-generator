using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Orders;

namespace Demo.Domain.Handlers.Orders
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Get order by id.
    /// Operation: GetOrderById.
    /// Area: Orders.
    /// </summary>
    public class GetOrderByIdHandler : IGetOrderByIdHandler
    {
        public Task<GetOrderByIdResult> ExecuteAsync(GetOrderByIdParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<GetOrderByIdResult> InvokeExecuteAsync(GetOrderByIdParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}