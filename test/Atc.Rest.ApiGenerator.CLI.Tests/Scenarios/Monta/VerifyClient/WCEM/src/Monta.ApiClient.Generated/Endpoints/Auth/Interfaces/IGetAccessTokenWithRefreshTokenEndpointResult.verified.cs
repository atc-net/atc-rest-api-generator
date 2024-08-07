﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.Auth.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Obtain your `accessToken` with a `refreshToken`.
/// Operation: GetAccessTokenWithRefreshToken.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetAccessTokenWithRefreshTokenEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    Token OkContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }
}