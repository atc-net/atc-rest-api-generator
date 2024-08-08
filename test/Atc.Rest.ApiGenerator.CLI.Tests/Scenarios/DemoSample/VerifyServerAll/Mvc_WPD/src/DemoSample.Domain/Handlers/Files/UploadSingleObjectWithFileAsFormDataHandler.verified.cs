namespace DemoSample.Domain.Handlers.Files;

/// <summary>
/// Handler for operation request.
/// Description: Upload a file as FormData.
/// Operation: UploadSingleObjectWithFileAsFormData.
/// </summary>
public sealed class UploadSingleObjectWithFileAsFormDataHandler : IUploadSingleObjectWithFileAsFormDataHandler
{
    public Task<UploadSingleObjectWithFileAsFormDataResult> ExecuteAsync(
        UploadSingleObjectWithFileAsFormDataParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for UploadSingleObjectWithFileAsFormDataHandler");
    }
}