﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace CodeStructure1.ApiClient.Generated.Users.MyEndpoints;

/// <summary>
/// Client Endpoint result.
/// Description: Update user by id.
/// Operation: UpdateUserById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UpdateUserByIdEndpointResult : EndpointResponse, IUpdateUserByIdEndpointResult
{
    public UpdateUserByIdEndpointResult(EndpointResponse response)
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

    public string? BadRequestContent
        => IsBadRequest && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsBadRequest property first.");

    public string? NotFoundContent
        => IsNotFound && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsNotFound property first.");

    public string? ConflictContent
        => IsConflict && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsConflict property first.");
}