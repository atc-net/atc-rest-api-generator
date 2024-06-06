//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExampleWithNsWithTask.Api.Generated.Endpoints;

/// <summary>
/// Endpoint definitions.
/// </summary>
[Authorize]
[ApiController]
[Route("/api/v1/tasks")]
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public sealed class TasksController : ControllerBase
{
    /// <summary>
    /// Description: Returns tasks.
    /// Operation: GetTasks.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Contracts.Tasks.Task>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> GetTasks(
        [FromServices] IGetTasksHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(cancellationToken);
}
