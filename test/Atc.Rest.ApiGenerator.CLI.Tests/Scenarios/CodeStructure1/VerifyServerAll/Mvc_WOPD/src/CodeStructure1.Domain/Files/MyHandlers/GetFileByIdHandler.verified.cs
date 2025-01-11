namespace CodeStructure1.Domain.Files.MyHandlers;

/// <summary>
/// Handler for operation request.
/// Description: Get File By Id.
/// Operation: GetFileById.
/// </summary>
public sealed class GetFileByIdHandler : IGetFileByIdHandler
{
    public Task<GetFileByIdResult> ExecuteAsync(
        GetFileByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetFileByIdHandler");
    }
}