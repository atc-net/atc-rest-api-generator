﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Wallets;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve your wallets.
/// Operation: GetWallets.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetWalletsParameters
{
    /// <summary>
    /// page number to retrieve (starts with 0).
    /// </summary>
    public int Page { get; set; } = 0;

    /// <summary>
    /// number of items per page (between 1 and 100, default 10).
    /// </summary>
    public int PerPage { get; set; } = 10;

    public int? TeamId { get; set; }

    public int? OperatorId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Page)}: {Page}, {nameof(PerPage)}: {PerPage}, {nameof(TeamId)}: {TeamId}, {nameof(OperatorId)}: {OperatorId}";
}