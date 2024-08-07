﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.WalletTransactions;

/// <summary>
/// Parameters for operation request.
/// Description: Patch an existing transaction.
/// Operation: PatchTransaction.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PatchTransactionParameters
{
    [Required]
    public long TransactionId { get; set; }

    [Required]
    public PatchWalletTransaction Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(TransactionId)}: {TransactionId}, {nameof(Request)}: ({Request})";
}