//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExampleWithUsers.ApiClient.Generated.Endpoints.Users.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Get all users.
/// Operation: GetUsers.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetUsersEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsUnauthorized { get; }

    bool IsConflict { get; }

    IEnumerable<User> OkContent { get; }

    ProblemDetails UnauthorizedContent { get; }

    ProblemDetails ConflictContent { get; }
}
