﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Plans;

/// <summary>
/// PlanServiceConfig.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PlanServiceConfig
{
    /// <summary>
    /// A list of PlanServiceConfig.
    /// </summary>
    public List<PlanServiceConfig> PlanServiceConfigList { get; set; } = new List<PlanServiceConfig>();

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(PlanServiceConfigList)}.Count: {PlanServiceConfigList?.Count ?? 0}";
}