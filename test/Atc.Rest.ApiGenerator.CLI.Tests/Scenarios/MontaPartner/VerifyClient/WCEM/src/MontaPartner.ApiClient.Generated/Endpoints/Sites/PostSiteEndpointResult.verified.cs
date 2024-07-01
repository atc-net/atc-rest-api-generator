﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.Sites;

/// <summary>
/// Client Endpoint result.
/// Description: Create a (charge) site.
/// Operation: PostSite.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PostSiteEndpointResult : EndpointResponse, IPostSiteEndpointResult
{
    public PostSiteEndpointResult(EndpointResponse response)
        : base(response)
    {
    }

    public bool IsCreated
        => StatusCode == HttpStatusCode.Created;

    public bool IsBadRequest
        => StatusCode == HttpStatusCode.BadRequest;

    public bool IsUnauthorized
        => StatusCode == HttpStatusCode.Unauthorized;

    public bool IsForbidden
        => StatusCode == HttpStatusCode.Forbidden;

    public string? CreatedContent
        => IsCreated && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsCreated property first.");

    public string? BadRequestContent
        => IsBadRequest && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsBadRequest property first.");

    public string? UnauthorizedContent
        => IsUnauthorized && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsUnauthorized property first.");

    public string? ForbiddenContent
        => IsForbidden && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsForbidden property first.");
}