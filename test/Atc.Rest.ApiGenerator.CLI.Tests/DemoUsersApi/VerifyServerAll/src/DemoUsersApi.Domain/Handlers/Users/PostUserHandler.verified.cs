namespace DemoUsersApi.Domain.Handlers.Users;

/// <summary>
/// Handler for operation request.
/// Description: Create a new user.
/// Operation: PostUser.
/// </summary>
public class PostUserHandler : IPostUserHandler
{
    public Task<PostUserResult> ExecuteAsync(
        PostUserParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for PostUserHandler");
    }
}