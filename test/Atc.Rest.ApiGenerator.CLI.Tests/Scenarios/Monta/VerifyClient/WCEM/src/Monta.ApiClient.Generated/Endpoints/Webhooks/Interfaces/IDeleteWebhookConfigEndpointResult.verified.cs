﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.Webhooks.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Delete webhook config.
/// Operation: DeleteWebhookConfig.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IDeleteWebhookConfigEndpointResult : IEndpointResponse
{

    bool IsNoContent { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    string? NoContentContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }
}