﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts;

/// <summary>
/// Enumeration: WebhookEntryEventType.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WebhookEntryEventType
{
    [EnumMember(Value = "*")]
    None,

    [EnumMember(Value = "charges")]
    Charges,

    [EnumMember(Value = "charge-points")]
    ChargePoints,

    [EnumMember(Value = "sites")]
    Sites,

    [EnumMember(Value = "team-members")]
    TeamMembers,

    [EnumMember(Value = "teams")]
    Teams,

    [EnumMember(Value = "installer-jobs")]
    InstallerJobs,

    [EnumMember(Value = "wallet-transactions")]
    WalletTransactions,

    [EnumMember(Value = "price-groups")]
    PriceGroups,

    [EnumMember(Value = "subscriptions")]
    Subscriptions,

    [EnumMember(Value = "plans")]
    Plans,
}