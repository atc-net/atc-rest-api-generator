//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Api.Generated.Contracts.Orders;

/// <summary>
/// Request to update an order.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UpdateOrderRequest
{
    [Required]
    [EmailAddress]
    public string MyEmail { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(MyEmail)}: {MyEmail}";
}