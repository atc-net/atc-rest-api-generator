﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace TestUnit.Task.NsApi.ApiClient.Generated.Endpoints;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Returns tasks.
/// Operation: GetTasks.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetTasksEndpointResult : IEndpointResponse
{
    bool IsOk { get; }

    bool IsUnauthorized { get; }

    bool IsForbidden { get; }

    bool IsInternalServerError { get; }

    List<Contracts.Task> OkContent { get; }

    string InternalServerErrorContent { get; }
}