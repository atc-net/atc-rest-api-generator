namespace Scenario2.Domain.Handlers.Orders
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Get orders.
    /// Operation: GetOrders.
    /// Area: Orders.
    /// </summary>
    public class GetOrdersHandler : IGetOrdersHandler
    {
        public Task<GetOrdersResult> ExecuteAsync(GetOrdersParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<GetOrdersResult> InvokeExecuteAsync(GetOrdersParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}