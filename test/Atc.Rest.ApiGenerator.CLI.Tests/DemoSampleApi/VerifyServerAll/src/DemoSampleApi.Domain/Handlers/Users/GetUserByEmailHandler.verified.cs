namespace DemoSampleApi.Domain.Handlers.Users;

/// <summary>
/// Handler for operation request.
/// Description: Get user by email.
/// Operation: GetUserByEmail.
/// </summary>
public class GetUserByEmailHandler : IGetUserByEmailHandler
{
    public Task<GetUserByEmailResult> ExecuteAsync(
        GetUserByEmailParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetUserByEmailHandler");
    }
}