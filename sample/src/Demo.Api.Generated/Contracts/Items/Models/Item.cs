﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Items;

/// <summary>
/// Item.
/// </summary>
[GeneratedCode("ApiGenerator", "2.0.259.29354")]
public class Item
{
    [Required]
    public string Name { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Name)}: {Name}";
}