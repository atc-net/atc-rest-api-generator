﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.PriceGroups;

/// <summary>
/// Parameters for operation request.
/// Description: Apply price group to charge points, sites or team members.
/// Operation: ApplyPriceGroup.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class ApplyPriceGroupParameters
{
    [Required]
    [Range(0, 2147483647)]
    public long Id { get; set; }

    [Required]
    public ApplyPriceGroup Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Request)}: ({Request})";
}