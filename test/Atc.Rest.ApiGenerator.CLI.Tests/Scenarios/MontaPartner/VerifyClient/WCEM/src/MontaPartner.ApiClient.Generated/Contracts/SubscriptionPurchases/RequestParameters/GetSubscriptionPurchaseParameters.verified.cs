﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.SubscriptionPurchases;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve a subscription purchase.
/// Operation: GetSubscriptionPurchase.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetSubscriptionPurchaseParameters
{
    [Required]
    public long SubscriptionPurchaseId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(SubscriptionPurchaseId)}: {SubscriptionPurchaseId}";
}