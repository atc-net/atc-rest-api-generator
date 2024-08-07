﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Teams;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve a list of teams.
/// Operation: GetTeams.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetTeamsParameters
{
    /// <summary>
    /// Filter teams by partnerExternalId, to filter only resources without `partnerExternalId` *use* `partnerExternalId=""`.
    /// </summary>
    public string PartnerExternalId { get; set; }

    /// <summary>
    /// Filter to include delete teams in the response.
    /// </summary>
    public bool IncludeDeleted { get; set; } = false;

    /// <summary>
    /// page number to retrieve (starts with 0).
    /// </summary>
    public int Page { get; set; } = 0;

    /// <summary>
    /// number of items per page (between 1 and 100, default 10).
    /// </summary>
    public int PerPage { get; set; } = 10;

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(PartnerExternalId)}: {PartnerExternalId}, {nameof(IncludeDeleted)}: {IncludeDeleted}, {nameof(Page)}: {Page}, {nameof(PerPage)}: {PerPage}";
}