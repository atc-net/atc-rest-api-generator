﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Wallets;

/// <summary>
/// WalletDtoBalanceDto.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class WalletDtoBalanceDto
{
    /// <summary>
    /// Amount of the balance.
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// Amount of the credit.
    /// </summary>
    public double Credit { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Amount)}: {Amount}, {nameof(Credit)}: {Credit}";
}