﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Plans;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve a plan.
/// Operation: GetPlan.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetPlanParameters
{
    [Required]
    public long Id { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}";
}