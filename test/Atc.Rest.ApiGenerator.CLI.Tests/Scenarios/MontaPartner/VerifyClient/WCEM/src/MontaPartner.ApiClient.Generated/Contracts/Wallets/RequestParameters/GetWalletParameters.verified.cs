﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Wallets;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve a single wallet.
/// Operation: GetWallet.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetWalletParameters
{
    [Required]
    public long WalletId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(WalletId)}: {WalletId}";
}