﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.ChargePointModels;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve list of charge point models.
/// Operation: GetChargePointModels.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetChargePointModelsParameters
{
    /// <summary>
    /// Optional filter to search models by model name or brand name.
    /// </summary>
    public string Search { get; set; }

    /// <summary>
    /// Optional filter to filter models by `brandId`.
    /// </summary>
    public long BrandId { get; set; }

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
        => $"{nameof(Search)}: {Search}, {nameof(BrandId)}: {BrandId}, {nameof(Page)}: {Page}, {nameof(PerPage)}: {PerPage}";
}