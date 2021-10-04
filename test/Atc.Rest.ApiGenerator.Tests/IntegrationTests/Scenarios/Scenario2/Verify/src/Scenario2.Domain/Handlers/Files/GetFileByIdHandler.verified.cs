using System.Threading;
using System.Threading.Tasks;
using Scenario2.Api.Generated.Contracts.Files;

namespace Scenario2.Domain.Handlers.Files
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Get File By Id.
    /// Operation: GetFileById.
    /// Area: Files.
    /// </summary>
    public class GetFileByIdHandler : IGetFileByIdHandler
    {
        public Task<GetFileByIdResult> ExecuteAsync(GetFileByIdParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<GetFileByIdResult> InvokeExecuteAsync(GetFileByIdParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}