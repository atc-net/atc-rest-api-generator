﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.197.51239.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Users;

/// <summary>
/// A single user.
/// </summary>
[GeneratedCode("ApiGenerator", "2.0.197.51239")]
public class User
{
    public Guid Id { get; set; }

    /// <summary>
    /// GenderType.
    /// </summary>
    public GenderType Gender { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    /// <summary>
    /// Undefined description.
    /// </summary>
    /// <remarks>
    /// Email validation being enforced.
    /// </remarks>
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// Undefined description.
    /// </summary>
    /// <remarks>
    /// Url validation being enforced.
    /// </remarks>
    [Uri]
    public Uri Homepage { get; set; }

    /// <summary>
    /// ColorType.
    /// </summary>
    public ColorType Color { get; set; }

    /// <summary>
    /// Address.
    /// </summary>
    public Address HomeAddress { get; set; }

    /// <summary>
    /// Address.
    /// </summary>
    public Address CompanyAddress { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Gender)}: ({Gender}), {nameof(FirstName)}: {FirstName}, {nameof(LastName)}: {LastName}, {nameof(Email)}: {Email}, {nameof(Homepage)}: {Homepage}, {nameof(Color)}: ({Color}), {nameof(HomeAddress)}: ({HomeAddress}), {nameof(CompanyAddress)}: ({CompanyAddress})";
    }
}