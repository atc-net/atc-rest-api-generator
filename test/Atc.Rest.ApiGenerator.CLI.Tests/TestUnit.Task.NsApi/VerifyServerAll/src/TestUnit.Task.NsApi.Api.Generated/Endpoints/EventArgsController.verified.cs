//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace TestUnit.Task.NsApi.Api.Generated.Endpoints;

/// <summary>
/// Endpoint definitions.
/// </summary>
[Authorize]
[ApiController]
[Route("/api/v1/eventArgs")]
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class EventArgsController : ControllerBase
{
    /// <summary>
    /// Description: Get EventArgs List.
    /// Operation: GetEventArgs.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TestUnit.Task.NsApi.Api.Generated.Contracts.EventArgs.EventArgs>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> GetEventArgs(
        [FromServices] IGetEventArgsHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(cancellationToken);

    /// <summary>
    /// Description: Get EventArgs By Id.
    /// Operation: GetEventArgById.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TestUnit.Task.NsApi.Api.Generated.Contracts.EventArgs.EventArgs), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetEventArgById(
        GetEventArgByIdParameters parameters,
        [FromServices] IGetEventArgByIdHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(parameters, cancellationToken);
}