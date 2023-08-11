namespace Demo.Domain.Handlers.Files;

/// <summary>
/// Handler for operation request.
/// Description: Upload multi files as form data.
/// Operation: UploadMultiFilesAsFormData.
/// </summary>
public class UploadMultiFilesAsFormDataHandler : IUploadMultiFilesAsFormDataHandler
{
    public Task<UploadMultiFilesAsFormDataResult> ExecuteAsync(
        UploadMultiFilesAsFormDataParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for UploadMultiFilesAsFormDataHandler");
    }
}