using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Files;

namespace Demo.Domain.Handlers.Files
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Upload a file as FormData.
    /// Operation: UploadFileAsFormData.
    /// Area: Files.
    /// </summary>
    public class UploadFileAsFormDataHandler : IUploadFileAsFormDataHandler
    {
        public Task<UploadFileAsFormDataResult> ExecuteAsync(UploadFileAsFormDataParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<UploadFileAsFormDataResult> InvokeExecuteAsync(UploadFileAsFormDataParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}