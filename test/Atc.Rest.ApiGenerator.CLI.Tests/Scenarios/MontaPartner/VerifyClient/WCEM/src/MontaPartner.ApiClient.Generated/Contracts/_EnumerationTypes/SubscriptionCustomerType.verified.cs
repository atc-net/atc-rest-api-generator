﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts;

/// <summary>
/// Enumeration: SubscriptionCustomerType.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubscriptionCustomerType
{
    [EnumMember(Value = "team")]
    Team,

    [EnumMember(Value = "charge-point")]
    ChargePoint,

    [EnumMember(Value = "operator")]
    Operator,

    [EnumMember(Value = "other")]
    Other,
}