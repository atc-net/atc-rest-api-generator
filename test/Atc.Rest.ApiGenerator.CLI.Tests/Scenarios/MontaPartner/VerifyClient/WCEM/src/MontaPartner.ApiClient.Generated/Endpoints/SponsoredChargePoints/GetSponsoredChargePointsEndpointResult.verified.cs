﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.SponsoredChargePoints;

/// <summary>
/// Client Endpoint result.
/// Description: Retrieve a list of sponsored charge points.
/// Operation: GetSponsoredChargePoints.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetSponsoredChargePointsEndpointResult : EndpointResponse, IGetSponsoredChargePointsEndpointResult
{
    public GetSponsoredChargePointsEndpointResult(EndpointResponse response)
        : base(response)
    {
    }

    public bool IsOk
        => StatusCode == HttpStatusCode.OK;

    public bool IsBadRequest
        => StatusCode == HttpStatusCode.BadRequest;

    public bool IsUnauthorized
        => StatusCode == HttpStatusCode.Unauthorized;

    public MontaPageSponsoredChargePointDto OkContent
        => IsOk && ContentObject is MontaPageSponsoredChargePointDto result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsOk property first.");

    public string? BadRequestContent
        => IsBadRequest && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsBadRequest property first.");

    public string? UnauthorizedContent
        => IsUnauthorized && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsUnauthorized property first.");
}