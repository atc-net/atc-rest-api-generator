//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace TestProject.AtcTest.Endpoints;

/// <summary>
/// Endpoint definitions.
/// Area: Items.
/// </summary>
[Authorize]
[ApiController]
[Route("/api/v1/items")]
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class ItemsController : ControllerBase
{
    /// <summary>
    /// Description: Get item by id.
    /// Operation: GetItemById.
    /// Area: Items.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public Task<ActionResult> GetItemByIdAsync(GetItemByIdParameters parameters, [FromServices] IGetItemByIdHandler handler, CancellationToken cancellationToken)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        return InvokeGetItemByIdAsync(parameters, handler, cancellationToken);
    }

    private static async Task<ActionResult> InvokeGetItemByIdAsync(GetItemByIdParameters parameters, IGetItemByIdHandler handler, CancellationToken cancellationToken)
    {
        return await handler.ExecuteAsync(parameters, cancellationToken);
    }
}
