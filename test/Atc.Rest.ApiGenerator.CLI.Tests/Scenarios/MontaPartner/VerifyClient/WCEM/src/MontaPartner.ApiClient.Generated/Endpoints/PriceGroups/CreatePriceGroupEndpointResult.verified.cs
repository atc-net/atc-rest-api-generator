﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.PriceGroups;

/// <summary>
/// Client Endpoint result.
/// Description: Creates a price group.
/// Operation: CreatePriceGroup.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class CreatePriceGroupEndpointResult : EndpointResponse, ICreatePriceGroupEndpointResult
{
    public CreatePriceGroupEndpointResult(EndpointResponse response)
        : base(response)
    {
    }

    public bool IsCreated
        => StatusCode == HttpStatusCode.Created;

    public bool IsBadRequest
        => StatusCode == HttpStatusCode.BadRequest;

    public bool IsUnauthorized
        => StatusCode == HttpStatusCode.Unauthorized;

    public bool IsNotFound
        => StatusCode == HttpStatusCode.NotFound;

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

    public string? NotFoundContent
        => IsNotFound && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsNotFound property first.");
}