﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Items;

/// <summary>
/// CreateItemRequest.
/// </summary>
[GeneratedCode("ApiGenerator", "2.0.259.29354")]
public class CreateItemRequest
{
    /// <summary>
    /// Item.
    /// </summary>
    [Required]
    public Item Item { get; set; }

    /// <summary>
    /// A list of Item.
    /// </summary>
    [Required]
    public List<Item> MyItems { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Item)}: ({Item}), {nameof(MyItems)}.Count: {MyItems?.Count ?? 0}";
}