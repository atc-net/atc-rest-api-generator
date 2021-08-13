using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Files;

namespace Demo.Domain.Handlers.Files
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Upload a file as OctetStream.
    /// Operation: UploadSingleFileAsFormData.
    /// Area: Files.
    /// </summary>
    public class UploadSingleFileAsFormDataHandler : IUploadSingleFileAsFormDataHandler
    {
        public Task<UploadSingleFileAsFormDataResult> ExecuteAsync(UploadSingleFileAsFormDataParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<UploadSingleFileAsFormDataResult> InvokeExecuteAsync(UploadSingleFileAsFormDataParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}