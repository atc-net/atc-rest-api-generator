﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts;

/// <summary>
/// SubscriptionServiceConfig.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class SubscriptionServiceConfig
{
    /// <summary>
    /// A list of SubscriptionServiceConfig.
    /// </summary>
    public List<SubscriptionServiceConfig> SubscriptionServiceConfigList { get; set; } = new List<SubscriptionServiceConfig>();

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(SubscriptionServiceConfigList)}.Count: {SubscriptionServiceConfigList?.Count ?? 0}";
}