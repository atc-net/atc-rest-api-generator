namespace CodeStructure1.Domain.Users.MyHandlers;

/// <summary>
/// Handler for operation request.
/// Description: Create a new user.
/// Operation: PostUser.
/// </summary>
public sealed class PostUserHandler : IPostUserHandler
{
    public Task<PostUserResult> ExecuteAsync(
        PostUserParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for PostUserHandler");
    }
}