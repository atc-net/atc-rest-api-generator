﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.ApiClient.Generated.Endpoints.Users;

/// <summary>
/// Client Endpoint result.
/// Description: Update gender on a user.
/// Operation: UpdateMyTestGender.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UpdateMyTestGenderEndpointResult : EndpointResponse, IUpdateMyTestGenderEndpointResult
{
    public UpdateMyTestGenderEndpointResult(EndpointResponse response)
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