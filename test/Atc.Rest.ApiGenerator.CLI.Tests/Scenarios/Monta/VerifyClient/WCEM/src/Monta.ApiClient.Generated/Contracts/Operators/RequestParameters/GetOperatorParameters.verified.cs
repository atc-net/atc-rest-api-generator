﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Operators;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve an operator.
/// Operation: GetOperator.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetOperatorParameters
{
    [Required]
    public long Id { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}";
}