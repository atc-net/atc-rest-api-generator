﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.PriceGroups;

/// <summary>
/// Parameters for operation request.
/// Description: Updates a price group.
/// Operation: UpdatePriceGroup.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UpdatePriceGroupParameters
{
    [Required]
    public long Id { get; set; }

    [Required]
    public CreateOrUpdatePriceGroupDto Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Request)}: ({Request})";
}