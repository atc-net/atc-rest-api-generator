﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.CountryAreas;

/// <summary>
/// CountryArea.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class CountryArea
{
    /// <summary>
    /// Id of the country area.
    /// </summary>
    [Required]
    public long Id { get; set; }

    /// <summary>
    /// Id of the country for the country area.
    /// </summary>
    [Required]
    public long CountryId { get; set; }

    /// <summary>
    /// Name of the country area.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// The external id of the country area.
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// The level of depth for this country area.
    /// </summary>
    [Required]
    public CountryAreaLevel Level { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(CountryId)}: {CountryId}, {nameof(Name)}: {Name}, {nameof(ExternalId)}: {ExternalId}, {nameof(Level)}: ({Level})";
}