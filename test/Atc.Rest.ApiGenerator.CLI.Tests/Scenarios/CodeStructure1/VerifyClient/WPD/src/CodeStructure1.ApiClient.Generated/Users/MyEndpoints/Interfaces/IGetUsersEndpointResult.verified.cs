﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace CodeStructure1.ApiClient.Generated.Users.MyEndpoints.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Get all users.
/// Operation: GetUsers.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetUsersEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsConflict { get; }

    IEnumerable<User> OkContent { get; }

    ProblemDetails ConflictContent { get; }
}