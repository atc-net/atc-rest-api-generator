﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace CodeStructure1.Api.Generated.Accounts.MyContracts;

/// <summary>
/// UpdateAccountRequest.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UpdateAccountRequest
{
    public string Name { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Name)}: {Name}";
}