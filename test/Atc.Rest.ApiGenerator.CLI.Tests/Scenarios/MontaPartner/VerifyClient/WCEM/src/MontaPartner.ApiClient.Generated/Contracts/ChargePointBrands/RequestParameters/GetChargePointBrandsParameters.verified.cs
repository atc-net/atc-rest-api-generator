﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.ChargePointBrands;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve list of charge point brands.
/// Operation: GetChargePointBrands.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetChargePointBrandsParameters
{
    /// <summary>
    /// Optional filter to search brands by name.
    /// </summary>
    public string Search { get; set; }

    /// <summary>
    /// page number to retrieve (starts with 0).
    /// </summary>
    public int Page { get; set; } = 0;

    /// <summary>
    /// number of items per page (between 1 and 100, default 10).
    /// </summary>
    public int PerPage { get; set; } = 10;

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Search)}: {Search}, {nameof(Page)}: {Page}, {nameof(PerPage)}: {PerPage}";
}