﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Insights;

/// <summary>
/// MontaPageChargesInsightChargerReportDto.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class MontaPageChargesInsightChargerReportDto
{
    [Required]
    public List<ChargesInsightChargerReport> Data { get; set; }

    [Required]
    public MontaPageMeta Meta { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Data)}.Count: {Data?.Count ?? 0}, {nameof(Meta)}: ({Meta})";
}