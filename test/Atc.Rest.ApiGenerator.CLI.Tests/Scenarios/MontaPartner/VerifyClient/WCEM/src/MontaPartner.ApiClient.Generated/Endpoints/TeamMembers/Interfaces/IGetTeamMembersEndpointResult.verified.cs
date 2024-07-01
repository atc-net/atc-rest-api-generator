﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.TeamMembers.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Retrieve a list of team members.
/// Operation: GetTeamMembers.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetTeamMembersEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    MontaPageTeamMemberDto OkContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }
}