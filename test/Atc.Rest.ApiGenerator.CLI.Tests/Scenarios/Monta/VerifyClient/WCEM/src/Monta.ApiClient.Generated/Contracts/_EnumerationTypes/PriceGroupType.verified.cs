﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts;

/// <summary>
/// Enumeration: PriceGroupType.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PriceGroupType
{
    [EnumMember(Value = "public")]
    Public,

    [EnumMember(Value = "member")]
    Member,

    [EnumMember(Value = "sponsored")]
    Sponsored,

    [EnumMember(Value = "cost")]
    Cost,

    [EnumMember(Value = "roaming")]
    Roaming,

    [EnumMember(Value = "reimbursement")]
    Reimbursement,

    [EnumMember(Value = "other")]
    Other,
}