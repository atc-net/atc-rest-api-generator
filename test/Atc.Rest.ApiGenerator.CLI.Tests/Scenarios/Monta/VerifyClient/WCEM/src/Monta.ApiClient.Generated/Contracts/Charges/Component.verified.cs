﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Charges;

/// <summary>
/// Component.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class Component
{
    /// <summary>
    /// The type of price/fee for this component.
    /// </summary>
    [Required]
    public ComponentTypeDto Type { get; set; }

    /// <summary>
    /// The price of this component.
    /// </summary>
    [Required]
    public long Price { get; set; }

    /// <summary>
    /// The readable description of this component.
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// Indicates this component is master a master pricing.
    /// </summary>
    [Required]
    public bool MasterPricing { get; set; }

    /// <summary>
    /// The tag for this component.
    /// </summary>
    public PriceGroupTag? Tag { get; set; }

    /// <summary>
    /// Additional information (metadata) for this component, e.g tariffId, kwh and others.
    /// </summary>
    public ComponentMetadataDto? Metadata { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Type)}: ({Type}), {nameof(Price)}: {Price}, {nameof(Description)}: {Description}, {nameof(MasterPricing)}: {MasterPricing}, {nameof(Tag)}: ({Tag}), {nameof(Metadata)}: ({Metadata})";
}