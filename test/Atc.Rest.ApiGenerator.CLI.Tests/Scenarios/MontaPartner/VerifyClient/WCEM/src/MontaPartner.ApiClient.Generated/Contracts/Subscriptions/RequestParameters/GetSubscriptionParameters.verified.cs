﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Subscriptions;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve a subscription.
/// Operation: GetSubscription.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetSubscriptionParameters
{
    [Required]
    public long SubscriptionId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(SubscriptionId)}: {SubscriptionId}";
}