using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Eventargs;

namespace Demo.Domain.Handlers.Eventargs
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Get EventArgs By Id.
    /// Operation: GetEventArgById.
    /// Area: Eventargs.
    /// </summary>
    public class GetEventArgByIdHandler : IGetEventArgByIdHandler
    {
        public Task<GetEventArgByIdResult> ExecuteAsync(GetEventArgByIdParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<GetEventArgByIdResult> InvokeExecuteAsync(GetEventArgByIdParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}