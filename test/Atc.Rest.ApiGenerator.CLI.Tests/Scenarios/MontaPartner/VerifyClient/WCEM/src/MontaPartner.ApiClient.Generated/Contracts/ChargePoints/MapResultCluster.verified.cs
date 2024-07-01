﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.ChargePoints;

/// <summary>
/// Cluster model.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class MapResultCluster
{
    [Required]
    public Coordinates Coordinates { get; set; }

    /// <summary>
    /// Number of entities (Charge points and sites) clustered.
    /// </summary>
    public long Count { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Coordinates)}: ({Coordinates}), {nameof(Count)}: {Count}";
}