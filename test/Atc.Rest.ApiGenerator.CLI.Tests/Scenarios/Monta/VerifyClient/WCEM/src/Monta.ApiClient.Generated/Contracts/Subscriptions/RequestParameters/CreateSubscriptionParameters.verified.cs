﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Subscriptions;

/// <summary>
/// Parameters for operation request.
/// Description: Create subscription.
/// Operation: CreateSubscription.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class CreateSubscriptionParameters
{
    [Required]
    public CreateSubscription Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Request)}: ({Request})";
}