﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts;

/// <summary>
/// Enumeration: TeamMemberState.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TeamMemberState
{
    [EnumMember(Value = "requested")]
    Requested,

    [EnumMember(Value = "invited")]
    Invited,

    [EnumMember(Value = "rejected")]
    Rejected,

    [EnumMember(Value = "accepted")]
    Accepted,

    [EnumMember(Value = "blocked")]
    Blocked,

    [EnumMember(Value = "expired")]
    Expired,

    [EnumMember(Value = "other")]
    Other,
}