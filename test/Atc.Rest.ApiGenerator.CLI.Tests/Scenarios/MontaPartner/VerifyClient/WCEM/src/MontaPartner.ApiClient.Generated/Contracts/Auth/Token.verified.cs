﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Auth;

/// <summary>
/// Token.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class Token
{
    /// <summary>
    /// Access token, required to interact with our endpoints.
    /// </summary>
    [Required]
    public string AccessToken { get; set; }

    /// <summary>
    /// Refresh token, required to get a new access token.
    /// </summary>
    [Required]
    public string RefreshToken { get; set; }

    /// <summary>
    /// Expiration date of access token (1 hour).
    /// </summary>
    [Required]
    public DateTimeOffset AccessTokenExpirationDate { get; set; }

    /// <summary>
    /// Expiration date of refresh token (24 hours).
    /// </summary>
    [Required]
    public DateTimeOffset RefreshTokenExpirationDate { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(AccessToken)}: {AccessToken}, {nameof(RefreshToken)}: {RefreshToken}, {nameof(AccessTokenExpirationDate)}: ({AccessTokenExpirationDate}), {nameof(RefreshTokenExpirationDate)}: ({RefreshTokenExpirationDate})";
}