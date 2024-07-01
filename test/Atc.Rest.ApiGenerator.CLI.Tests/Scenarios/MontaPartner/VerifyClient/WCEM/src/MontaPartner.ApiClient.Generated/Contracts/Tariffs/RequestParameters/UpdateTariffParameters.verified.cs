﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Tariffs;

/// <summary>
/// Parameters for operation request.
/// Description: Update an existing Tariff.
/// Operation: UpdateTariff.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UpdateTariffParameters
{
    [Required]
    public long TariffId { get; set; }

    [Required]
    public UpdateTariffRequest Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(TariffId)}: {TariffId}, {nameof(Request)}: ({Request})";
}