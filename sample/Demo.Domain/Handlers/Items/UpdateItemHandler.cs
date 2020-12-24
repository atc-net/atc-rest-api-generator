using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Items;

namespace Demo.Domain.Handlers.Items
{
    /// <summary>
    /// Handler for operation request.
    /// Description: .
    /// Operation: UpdateItem.
    /// Area: Items.
    /// </summary>
    public class UpdateItemHandler : IUpdateItemHandler
    {
        public Task<UpdateItemResult> ExecuteAsync(UpdateItemParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

        private Task<UpdateItemResult> InvokeExecuteAsync(UpdateItemParameters parameters, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}