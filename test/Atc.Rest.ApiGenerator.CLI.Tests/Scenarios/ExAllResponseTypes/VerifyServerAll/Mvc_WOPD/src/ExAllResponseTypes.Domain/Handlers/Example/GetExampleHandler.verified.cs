namespace ExAllResponseTypes.Domain.Handlers.Example;

/// <summary>
/// Handler for operation request.
/// Description: Example endpoint.
/// Operation: GetExample.
/// </summary>
public sealed class GetExampleHandler : IGetExampleHandler
{
    public Task<GetExampleResult> ExecuteAsync(
        GetExampleParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetExampleHandler");
    }
}