﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts;

/// <summary>
/// CurrencyDto.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class CurrencyDto
{
    /// <summary>
    /// id of the currency.
    /// </summary>
    [Range(0, int.MaxValue)]
    public long? Id { get; set; }

    /// <summary>
    /// Currency identifier.
    /// </summary>
    public string? Identifier { get; set; }

    /// <summary>
    /// Readable name of currency.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Number of decimals for this currency.
    /// </summary>
    public int Decimals { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Identifier)}: {Identifier}, {nameof(Name)}: {Name}, {nameof(Decimals)}: {Decimals}";
}