namespace Scenario2.Domain.Handlers.Files
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Upload a file as FormData.
    /// Operation: UploadSingleObjectWithFileAsFormData.
    /// Area: Files.
    /// </summary>
    public class UploadSingleObjectWithFileAsFormDataHandler : IUploadSingleObjectWithFileAsFormDataHandler
    {
        public Task<UploadSingleObjectWithFileAsFormDataResult> ExecuteAsync(UploadSingleObjectWithFileAsFormDataParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<UploadSingleObjectWithFileAsFormDataResult> InvokeExecuteAsync(UploadSingleObjectWithFileAsFormDataParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}