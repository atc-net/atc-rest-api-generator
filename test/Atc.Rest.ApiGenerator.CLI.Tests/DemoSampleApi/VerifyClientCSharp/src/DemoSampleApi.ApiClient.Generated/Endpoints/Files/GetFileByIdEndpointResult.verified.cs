﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSampleApi.ApiClient.Generated.Endpoints;

/// <summary>
/// Client Endpoint result.
/// Description: Get File By Id.
/// Operation: GetFileById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetFileByIdEndpointResult : EndpointResponse, IGetFileByIdEndpointResult
{
    public GetFileByIdEndpointResult(EndpointResponse response)
    : base(response)
    {
    }

    public bool IsOk
        => StatusCode == HttpStatusCode.OK;

    public bool IsBadRequest
        => StatusCode == HttpStatusCode.BadRequest;

    public bool IsUnauthorized
        => StatusCode == HttpStatusCode.Unauthorized;

    public bool IsForbidden
        => StatusCode == HttpStatusCode.Forbidden;

    public bool IsNotFound
        => StatusCode == HttpStatusCode.NotFound;

    public bool IsInternalServerError
        => StatusCode == HttpStatusCode.InternalServerError;

    public string OkContent
        => IsOk && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsOk property first.");

    public ValidationProblemDetails BadRequestContent
        => IsBadRequest && ContentObject is ValidationProblemDetails result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsBadRequest property first.");

    public string NotFoundContent
        => IsNotFound && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsNotFound property first.");

    public string InternalServerErrorContent
        => IsInternalServerError && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsInternalServerError property first.");
}