﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSampleApi.ApiClient.Generated.Endpoints;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Set name of account.
/// Operation: SetAccountName.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface ISetAccountNameEndpointResult : IEndpointResponse
{
    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    bool IsForbidden { get; }

    bool IsInternalServerError { get; }

    string OkContent { get; }

    ValidationProblemDetails BadRequestContent { get; }

    string InternalServerErrorContent { get; }
}