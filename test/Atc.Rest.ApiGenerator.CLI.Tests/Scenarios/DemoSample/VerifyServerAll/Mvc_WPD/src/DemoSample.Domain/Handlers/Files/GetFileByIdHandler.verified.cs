namespace DemoSample.Domain.Handlers.Files;

/// <summary>
/// Handler for operation request.
/// Description: Get File By Id.
/// Operation: GetFileById.
/// </summary>
public class GetFileByIdHandler : IGetFileByIdHandler
{
    public Task<GetFileByIdResult> ExecuteAsync(
        GetFileByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetFileByIdHandler");
    }
}