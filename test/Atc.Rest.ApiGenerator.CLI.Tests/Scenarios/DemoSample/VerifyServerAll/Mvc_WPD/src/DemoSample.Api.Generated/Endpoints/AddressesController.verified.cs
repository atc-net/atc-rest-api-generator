﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.Api.Generated.Endpoints;

/// <summary>
/// Endpoint definitions.
/// </summary>
[ApiController]
[Route("/api/v1/addresses")]
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public sealed class AddressesController : ControllerBase
{
    /// <summary>
    /// Description: Get addresses by postal code.
    /// Operation: GetAddressesByPostalCodes.
    /// </summary>
    [HttpGet("{postalCode}")]
    [ProducesResponseType(typeof(IEnumerable<Address>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetAddressesByPostalCodes(
        GetAddressesByPostalCodesParameters parameters,
        [FromServices] IGetAddressesByPostalCodesHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(parameters, cancellationToken);
}