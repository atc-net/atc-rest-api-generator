﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace CodeStructure1.ApiClient.Generated.Addresses.MyContracts;

/// <summary>
/// Parameters for operation request.
/// Description: Get addresses by postal code.
/// Operation: GetAddressesByPostalCodes.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetAddressesByPostalCodesParameters
{
    /// <summary>
    /// The postalCode to limit addresses on.
    /// </summary>
    [Required]
    public string PostalCode { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(PostalCode)}: {PostalCode}";
}