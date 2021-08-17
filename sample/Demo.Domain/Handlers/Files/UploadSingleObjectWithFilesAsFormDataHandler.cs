using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Files;

namespace Demo.Domain.Handlers.Files
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Upload files as FormData.
    /// Operation: UploadSingleObjectWithFilesAsFormData.
    /// Area: Files.
    /// </summary>
    public class UploadSingleObjectWithFilesAsFormDataHandler : IUploadSingleObjectWithFilesAsFormDataHandler
    {
        public Task<UploadSingleObjectWithFilesAsFormDataResult> ExecuteAsync(UploadSingleObjectWithFilesAsFormDataParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<UploadSingleObjectWithFilesAsFormDataResult> InvokeExecuteAsync(UploadSingleObjectWithFilesAsFormDataParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}