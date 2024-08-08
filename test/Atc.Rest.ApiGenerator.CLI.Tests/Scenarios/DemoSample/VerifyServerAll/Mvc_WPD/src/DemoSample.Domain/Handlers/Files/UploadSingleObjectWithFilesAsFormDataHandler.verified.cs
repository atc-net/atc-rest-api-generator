namespace DemoSample.Domain.Handlers.Files;

/// <summary>
/// Handler for operation request.
/// Description: Upload files as FormData.
/// Operation: UploadSingleObjectWithFilesAsFormData.
/// </summary>
public sealed class UploadSingleObjectWithFilesAsFormDataHandler : IUploadSingleObjectWithFilesAsFormDataHandler
{
    public Task<UploadSingleObjectWithFilesAsFormDataResult> ExecuteAsync(
        UploadSingleObjectWithFilesAsFormDataParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for UploadSingleObjectWithFilesAsFormDataHandler");
    }
}