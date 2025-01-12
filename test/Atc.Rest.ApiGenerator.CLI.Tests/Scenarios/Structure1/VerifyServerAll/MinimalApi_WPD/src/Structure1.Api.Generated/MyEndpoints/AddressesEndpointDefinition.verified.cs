﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Structure1.Api.Generated.MyEndpoints;

/// <summary>
/// Endpoint definitions.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public sealed class AddressesEndpointDefinition : IEndpointDefinition
{
    internal const string ApiRouteBase = "/api/v1/addresses";

    public void DefineEndpoints(
        WebApplication app)
    {
        var addresses = app
            .NewVersionedApi("Addresses")
            .MapGroup(ApiRouteBase);

        addresses
            .MapGet("{postalCode}", GetAddressesByPostalCodes)
            .WithName("GetAddressesByPostalCodes")
            .WithSummary("Get addresses by postal code.")
            .WithDescription("Get addresses by postal code.")
            .AddEndpointFilter<ValidationFilter<GetAddressesByPostalCodesParameters>>()
            .Produces<IEnumerable<Address>>()
            .ProducesValidationProblem()
            .Produces<string?>(StatusCodes.Status404NotFound);
    }

    internal async Task<IResult> GetAddressesByPostalCodes(
        [FromServices] IGetAddressesByPostalCodesHandler handler,
        [AsParameters] GetAddressesByPostalCodesParameters parameters,
        CancellationToken cancellationToken)
        => GetAddressesByPostalCodesResult.ToIResult(
            await handler.ExecuteAsync(
                parameters,
                cancellationToken));
}