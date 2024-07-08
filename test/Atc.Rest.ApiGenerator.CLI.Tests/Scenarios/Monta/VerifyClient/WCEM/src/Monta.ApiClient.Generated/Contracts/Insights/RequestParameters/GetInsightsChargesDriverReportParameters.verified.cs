﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Insights;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve insights about charges broken down by team members of a team.
/// Operation: GetInsightsChargesDriverReport.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetInsightsChargesDriverReportParameters
{
    [Required]
    public long TeamId { get; set; }

    public string? TeamMemberIds { get; set; }

    [Required]
    public DateTimeOffset FromDate { get; set; }

    [Required]
    public DateTimeOffset ToDate { get; set; }

    public DriverReportDatesFilteredBy? DatesFilteredBy { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(TeamId)}: {TeamId}, {nameof(TeamMemberIds)}: {TeamMemberIds}, {nameof(FromDate)}: ({FromDate}), {nameof(ToDate)}: ({ToDate}), {nameof(DatesFilteredBy)}: ({DatesFilteredBy})";
}