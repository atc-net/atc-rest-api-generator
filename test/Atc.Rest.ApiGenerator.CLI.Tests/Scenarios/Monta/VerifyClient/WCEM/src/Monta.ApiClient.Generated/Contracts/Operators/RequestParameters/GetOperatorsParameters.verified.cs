﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Operators;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve operators.
/// Operation: GetOperators.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetOperatorsParameters
{
    public int? Page { get; set; } = 0;

    public int? PerPage { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Page)}: {Page}, {nameof(PerPage)}: {PerPage}";
}