﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts;

/// <summary>
/// Enumeration: ChargePointIntegrationStateDto.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChargePointIntegrationStateDto
{
    [EnumMember(Value = "pending")]
    Pending,

    [EnumMember(Value = "connected")]
    Connected,

    [EnumMember(Value = "disconnected")]
    Disconnected,

    [EnumMember(Value = "error")]
    Error,

    [EnumMember(Value = "unknown")]
    Unknown,
}