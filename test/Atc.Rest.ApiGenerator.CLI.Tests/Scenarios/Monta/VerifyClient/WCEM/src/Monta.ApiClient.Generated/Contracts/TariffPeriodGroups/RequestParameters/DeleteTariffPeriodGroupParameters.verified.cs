﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.TariffPeriodGroups;

/// <summary>
/// Parameters for operation request.
/// Description: Delete an existing Tariff Period Group, and all contained recurring periods and prices.
/// Operation: DeleteTariffPeriodGroup.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class DeleteTariffPeriodGroupParameters
{
    [Required]
    public long Id { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}";
}