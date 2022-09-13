namespace Scenario2.Domain.Handlers.Orders
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Update part of order by id.
    /// Operation: PatchOrdersId.
    /// Area: Orders.
    /// </summary>
    public class PatchOrdersIdHandler : IPatchOrdersIdHandler
    {
        public Task<PatchOrdersIdResult> ExecuteAsync(PatchOrdersIdParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<PatchOrdersIdResult> InvokeExecuteAsync(PatchOrdersIdParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}