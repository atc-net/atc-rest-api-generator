﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Invoices;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieves a list of invoices by walletId.
/// Operation: GetInvoice.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetInvoiceParameters
{
    /// <summary>
    /// page number to retrieve (starts with 0).
    /// </summary>
    public int Page { get; set; } = 0;

    /// <summary>
    /// number of items per page (between 1 and 100, default 10).
    /// </summary>
    public int PerPage { get; set; } = 10;

    [Required]
    public long WalletId { get; set; }

    [Required]
    public Pageable Pageable { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Page)}: {Page}, {nameof(PerPage)}: {PerPage}, {nameof(WalletId)}: {WalletId}, {nameof(Pageable)}: ({Pageable})";
}