﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.ChargePoints;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve a list of charge points.
/// Operation: GetChargePoints.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetChargePointsParameters
{
    /// <summary>
    /// page number to retrieve (starts with 0).
    /// </summary>
    public int Page { get; set; } = 0;

    /// <summary>
    /// number of items per page (between 1 and 100, default 10).
    /// </summary>
    public int PerPage { get; set; } = 10;

    public long? SiteId { get; set; }

    public long? TeamId { get; set; }

    public string? PartnerExternalId { get; set; }

    [RegularExpression("^(-?\\d*\\.\\d+|\\d+\\.\\d*)(,)(-?\\d*\\.\\d+|\\d+\\.\\d*)$")]
    public string? SortByLocation { get; set; }

    public bool? IncludeDeleted { get; set; }

    public ChargePointState? State { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Page)}: {Page}, {nameof(PerPage)}: {PerPage}, {nameof(SiteId)}: {SiteId}, {nameof(TeamId)}: {TeamId}, {nameof(PartnerExternalId)}: {PartnerExternalId}, {nameof(SortByLocation)}: {SortByLocation}, {nameof(IncludeDeleted)}: {IncludeDeleted}, {nameof(State)}: ({State})";
}