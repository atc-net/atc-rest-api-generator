﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExNsWithTask.ApiClient.Generated.Endpoints.EventArgs.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Get EventArgs By Id.
/// Operation: GetEventArgById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetEventArgByIdEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsNotFound { get; }

    ExNsWithTask.ApiClient.Generated.Contracts.EventArgs.EventArgs OkContent { get; }

    string? BadRequestContent { get; }

    string? NotFoundContent { get; }
}