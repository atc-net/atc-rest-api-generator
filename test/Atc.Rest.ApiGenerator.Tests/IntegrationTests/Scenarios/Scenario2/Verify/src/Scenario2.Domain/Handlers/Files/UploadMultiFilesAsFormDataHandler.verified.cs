namespace Scenario2.Domain.Handlers.Files;

/// <summary>
/// Handler for operation request.
/// Description: Upload multi files as form data.
/// Operation: UploadMultiFilesAsFormData.
/// Area: Files.
/// </summary>
public class UploadMultiFilesAsFormDataHandler : IUploadMultiFilesAsFormDataHandler
{
    public Task<UploadMultiFilesAsFormDataResult> ExecuteAsync(UploadMultiFilesAsFormDataParameters parameters, CancellationToken cancellationToken = default)
    {
        if (parameters is null)
        {
            throw new System.ArgumentNullException(nameof(parameters));
        }

        return InvokeExecuteAsync(parameters, cancellationToken);
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async Task<UploadMultiFilesAsFormDataResult> InvokeExecuteAsync(UploadMultiFilesAsFormDataParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        throw new System.NotImplementedException();
    }
}