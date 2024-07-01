﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Currencies;

/// <summary>
/// CurrencyDto4.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class CurrencyDto4
{
    /// <summary>
    /// id of the currency.
    /// </summary>
    [Range(0, 2147483647)]
    public long? Id { get; set; }

    /// <summary>
    /// Currency identifier, e.g. DKK.
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