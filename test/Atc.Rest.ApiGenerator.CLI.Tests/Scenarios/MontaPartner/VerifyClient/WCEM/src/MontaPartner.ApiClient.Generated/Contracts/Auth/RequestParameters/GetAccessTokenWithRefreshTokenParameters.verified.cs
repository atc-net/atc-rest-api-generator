﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Auth;

/// <summary>
/// Parameters for operation request.
/// Description: Obtain your `accessToken` with a `refreshToken`.
/// Operation: GetAccessTokenWithRefreshToken.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetAccessTokenWithRefreshTokenParameters
{
    [Required]
    public string? Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Request)}: ({Request})";
}