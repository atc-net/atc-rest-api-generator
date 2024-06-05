//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSampleApi.Api.Generated.Contracts.Orders;

/// <summary>
/// Results for operation request.
/// Description: Update part of order by id.
/// Operation: PatchOrdersId.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PatchOrdersIdResult : ResultBase
{
    private PatchOrdersIdResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static PatchOrdersIdResult Ok(string? message = null)
        => new PatchOrdersIdResult(new OkObjectResult(message));

    /// <summary>
    /// 404 - NotFound response.
    /// </summary>
    public static PatchOrdersIdResult NotFound(string? message = null)
        => new PatchOrdersIdResult(new NotFoundObjectResult(message));

    /// <summary>
    /// 409 - Conflict response.
    /// </summary>
    public static PatchOrdersIdResult Conflict(string? message = null)
        => new PatchOrdersIdResult(new ConflictObjectResult(message));

    /// <summary>
    /// 502 - BadGateway response.
    /// </summary>
    public static PatchOrdersIdResult BadGateway(string? message = null)
        => new PatchOrdersIdResult(ResultFactory.CreateContentResult(HttpStatusCode.BadGateway, message));

    /// <summary>
    /// Performs an implicit conversion from PatchOrdersIdResult to ActionResult.
    /// </summary>
    public static implicit operator PatchOrdersIdResult(string response)
        => Ok(response);
}