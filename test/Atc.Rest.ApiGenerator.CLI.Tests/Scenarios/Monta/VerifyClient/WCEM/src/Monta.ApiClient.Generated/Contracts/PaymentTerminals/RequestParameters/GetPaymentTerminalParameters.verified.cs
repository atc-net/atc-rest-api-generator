﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.PaymentTerminals;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve a single payment terminal.
/// Operation: GetPaymentTerminal.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetPaymentTerminalParameters
{
    [Required]
    public string PaymentTerminalId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(PaymentTerminalId)}: {PaymentTerminalId}";
}