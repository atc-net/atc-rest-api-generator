namespace DemoSampleApi.Domain.Handlers.Files;

/// <summary>
/// Handler for operation request.
/// Description: Upload a file as FormData.
/// Operation: UploadSingleObjectWithFileAsFormData.
/// </summary>
public class UploadSingleObjectWithFileAsFormDataHandler : IUploadSingleObjectWithFileAsFormDataHandler
{
    public Task<UploadSingleObjectWithFileAsFormDataResult> ExecuteAsync(
        UploadSingleObjectWithFileAsFormDataParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for UploadSingleObjectWithFileAsFormDataHandler");
    }
}