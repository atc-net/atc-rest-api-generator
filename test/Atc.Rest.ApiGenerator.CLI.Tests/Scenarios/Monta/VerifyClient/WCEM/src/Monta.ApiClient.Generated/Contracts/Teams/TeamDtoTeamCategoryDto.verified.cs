﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Teams;

/// <summary>
/// TeamDtoTeamCategoryDto.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class TeamDtoTeamCategoryDto
{
    /// <summary>
    /// Category Identifier.
    /// </summary>
    [Required]
    public string Identifier { get; set; }

    /// <summary>
    /// Category name.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Identifier)}: {Identifier}, {nameof(Name)}: {Name}";
}