﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExUsers.ApiClient.Generated.Endpoints.Users;

/// <summary>
/// Client Endpoint result.
/// Description: Delete user by id.
/// Operation: DeleteUserById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class DeleteUserByIdEndpointResult : EndpointResponse, IDeleteUserByIdEndpointResult
{
    public DeleteUserByIdEndpointResult(EndpointResponse response)
        : base(response)
    {
    }

    public bool IsOk
        => StatusCode == HttpStatusCode.OK;

    public bool IsBadRequest
        => StatusCode == HttpStatusCode.BadRequest;

    public bool IsNotFound
        => StatusCode == HttpStatusCode.NotFound;

    public bool IsConflict
        => StatusCode == HttpStatusCode.Conflict;

    public string? OkContent
        => IsOk && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsOk property first.");

    public ValidationProblemDetails BadRequestContent
        => IsBadRequest && ContentObject is ValidationProblemDetails result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsBadRequest property first.");

    public string? NotFoundContent
        => IsNotFound && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsNotFound property first.");

    public ProblemDetails ConflictContent
        => IsConflict && ContentObject is ProblemDetails result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsConflict property first.");
}