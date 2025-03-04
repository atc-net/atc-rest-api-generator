﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.WalletTransactions;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieves wallet transactions for an invoice.
/// Operation: GetWalletTransactions1.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetWalletTransactions1Parameters
{
    [Required]
    public long InvoiceId { get; set; }

    [Required]
    public Pageable Pageable { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(InvoiceId)}: {InvoiceId}, {nameof(Pageable)}: ({Pageable})";
}