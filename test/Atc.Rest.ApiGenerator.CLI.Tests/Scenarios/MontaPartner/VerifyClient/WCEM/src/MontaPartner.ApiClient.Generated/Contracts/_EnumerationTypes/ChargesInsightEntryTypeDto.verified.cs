﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts;

/// <summary>
/// Enumeration: ChargesInsightEntryTypeDto.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChargesInsightEntryTypeDto
{
    [EnumMember(Value = "total-completed-sponsored-charges")]
    TotalCompletedSponsoredCharges,

    [EnumMember(Value = "total-completed-operator-charges")]
    TotalCompletedOperatorCharges,

    [EnumMember(Value = "total-completed-external-charges")]
    TotalCompletedExternalCharges,

    [EnumMember(Value = "total-completed-charges")]
    TotalCompletedCharges,

    [EnumMember(Value = "unknown")]
    Unknown,
}