namespace DemoSample.Domain.Handlers.Files;

/// <summary>
/// Handler for operation request.
/// Description: Upload a file as OctetStream.
/// Operation: UploadSingleFileAsFormData.
/// </summary>
public sealed class UploadSingleFileAsFormDataHandler : IUploadSingleFileAsFormDataHandler
{
    public Task<UploadSingleFileAsFormDataResult> ExecuteAsync(
        UploadSingleFileAsFormDataParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for UploadSingleFileAsFormDataHandler");
    }
}