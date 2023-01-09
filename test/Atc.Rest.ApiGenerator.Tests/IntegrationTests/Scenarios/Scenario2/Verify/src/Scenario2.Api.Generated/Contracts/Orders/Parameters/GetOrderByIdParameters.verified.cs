//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Api.Generated.Contracts.Orders;

/// <summary>
/// Parameters for operation request.
/// Description: Get order by id.
/// Operation: GetOrderById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetOrderByIdParameters
{
    /// <summary>
    /// The id of the order.
    /// </summary>
    [FromRoute(Name = "id")]
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The email for filter orders to retrieve.
    /// </summary>
    /// <remarks>
    /// Email validation being enforced.
    /// </remarks>
    [FromQuery(Name = "myEmail")]
    [EmailAddress]
    public string? MyEmail { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(MyEmail)}: {MyEmail}";
}