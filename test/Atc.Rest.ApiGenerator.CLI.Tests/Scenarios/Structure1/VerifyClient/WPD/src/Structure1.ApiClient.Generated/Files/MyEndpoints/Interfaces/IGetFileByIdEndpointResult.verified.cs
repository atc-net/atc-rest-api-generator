﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Structure1.ApiClient.Generated.Files.MyEndpoints.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Get File By Id.
/// Operation: GetFileById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetFileByIdEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsNotFound { get; }

    byte[] OkContent { get; }

    ValidationProblemDetails BadRequestContent { get; }

    string? NotFoundContent { get; }
}