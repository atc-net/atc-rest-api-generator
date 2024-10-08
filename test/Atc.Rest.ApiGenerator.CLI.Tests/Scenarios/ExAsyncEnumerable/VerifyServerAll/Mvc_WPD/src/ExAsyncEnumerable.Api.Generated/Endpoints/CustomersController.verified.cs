﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExAsyncEnumerable.Api.Generated.Endpoints;

/// <summary>
/// Endpoint definitions.
/// </summary>
[ApiController]
[Route("/api/v1/customers")]
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public sealed class CustomersController : ControllerBase
{
    /// <summary>
    /// Description: Get customers.
    /// Operation: GetCustomers.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IAsyncEnumerable<PaginationResult<Customer>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetCustomers(
        GetCustomersParameters parameters,
        [FromServices] IGetCustomersHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(parameters, cancellationToken);
}