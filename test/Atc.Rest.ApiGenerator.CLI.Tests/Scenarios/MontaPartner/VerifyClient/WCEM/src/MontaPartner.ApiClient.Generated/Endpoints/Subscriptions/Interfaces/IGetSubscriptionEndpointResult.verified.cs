﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.Subscriptions.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Retrieve a subscription.
/// Operation: GetSubscription.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetSubscriptionEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    Subscription OkContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }
}