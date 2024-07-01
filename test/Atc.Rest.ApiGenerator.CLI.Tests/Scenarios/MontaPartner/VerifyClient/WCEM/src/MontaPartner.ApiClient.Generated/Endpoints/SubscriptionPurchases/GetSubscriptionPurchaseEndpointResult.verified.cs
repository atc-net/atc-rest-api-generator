﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.SubscriptionPurchases;

/// <summary>
/// Client Endpoint result.
/// Description: Retrieve a subscription purchase.
/// Operation: GetSubscriptionPurchase.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetSubscriptionPurchaseEndpointResult : EndpointResponse, IGetSubscriptionPurchaseEndpointResult
{
    public GetSubscriptionPurchaseEndpointResult(EndpointResponse response)
        : base(response)
    {
    }

    public bool IsOk
        => StatusCode == HttpStatusCode.OK;

    public bool IsBadRequest
        => StatusCode == HttpStatusCode.BadRequest;

    public bool IsUnauthorized
        => StatusCode == HttpStatusCode.Unauthorized;

    public SubscriptionPurchase OkContent
        => IsOk && ContentObject is SubscriptionPurchase result
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