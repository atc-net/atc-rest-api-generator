using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Items;

namespace Demo.Domain.Handlers.Items
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Create a new item.
    /// Operation: CreateItem.
    /// Area: Items.
    /// </summary>
    public class CreateItemHandler : ICreateItemHandler
    {
        public Task<CreateItemResult> ExecuteAsync(CreateItemParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<CreateItemResult> InvokeExecuteAsync(CreateItemParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}