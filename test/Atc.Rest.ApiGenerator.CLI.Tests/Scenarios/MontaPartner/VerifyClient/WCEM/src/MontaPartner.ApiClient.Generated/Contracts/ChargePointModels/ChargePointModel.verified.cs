﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.ChargePointModels;

/// <summary>
/// ChargePointModel.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class ChargePointModel
{
    /// <summary>
    /// The id of the charge point model.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The identifier of the charge point model.
    /// </summary>
    [Required]
    public string Identifier { get; set; }

    /// <summary>
    /// The name of the charge point model, name is composed by (Brand name - Model name).
    /// </summary>
    [Required]
    public string Name { get; set; }

    [Required]
    public ChargePointModelDtoChargePointBrandDto Brand { get; set; }

    /// <summary>
    /// The supported features by this charge point model, only supported features will be included.
    /// </summary>
    [Required]
    public List<ChargePointModelFeature> Features { get; set; }

    /// <summary>
    /// Creation date of this charge point model.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Update date of this charge point model.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Identifier)}: {Identifier}, {nameof(Name)}: {Name}, {nameof(Brand)}: ({Brand}), {nameof(Features)}.Count: {Features?.Count ?? 0}, {nameof(CreatedAt)}: ({CreatedAt}), {nameof(UpdatedAt)}: ({UpdatedAt})";
}