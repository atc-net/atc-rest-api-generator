namespace CodeStructure1.Domain.Files.MyHandlers;

/// <summary>
/// Handler for operation request.
/// Description: Upload multi files as form data.
/// Operation: UploadMultiFilesAsFormData.
/// </summary>
public sealed class UploadMultiFilesAsFormDataHandler : IUploadMultiFilesAsFormDataHandler
{
    public Task<UploadMultiFilesAsFormDataResult> ExecuteAsync(
        UploadMultiFilesAsFormDataParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for UploadMultiFilesAsFormDataHandler");
    }
}