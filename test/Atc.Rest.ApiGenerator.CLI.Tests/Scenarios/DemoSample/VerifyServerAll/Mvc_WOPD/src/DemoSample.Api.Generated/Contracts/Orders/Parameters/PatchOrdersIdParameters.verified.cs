//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.Api.Generated.Contracts.Orders;

/// <summary>
/// Parameters for operation request.
/// Description: Update part of order by id.
/// Operation: PatchOrdersId.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PatchOrdersIdParameters
{
    /// <summary>
    /// The id of the order.
    /// </summary>
    [FromRoute(Name = "id")]
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The myTestHeader special key.
    /// </summary>
    [FromHeader(Name = "myTestHeader")]
    [Required]
    public string MyTestHeader { get; set; }

    /// <summary>
    /// The myTestHeaderBool special key.
    /// </summary>
    [FromHeader(Name = "myTestHeaderBool")]
    [Required]
    public bool MyTestHeaderBool { get; set; }

    /// <summary>
    /// The myTestHeaderInt special key.
    /// </summary>
    [FromHeader(Name = "myTestHeaderInt")]
    [Required]
    public int MyTestHeaderInt { get; set; }

    /// <summary>
    /// The correlationId.
    /// </summary>
    [FromHeader(Name = "x-correlation-id")]
    [Required]
    public string CorrelationId { get; set; }

    /// <summary>
    /// Request to update an order.
    /// </summary>
    [FromBody]
    [Required]
    public UpdateOrderRequest Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(MyTestHeader)}: {MyTestHeader}, {nameof(MyTestHeaderBool)}: {MyTestHeaderBool}, {nameof(MyTestHeaderInt)}: {MyTestHeaderInt}, {nameof(CorrelationId)}: {CorrelationId}, {nameof(Request)}: ({Request})";
}
