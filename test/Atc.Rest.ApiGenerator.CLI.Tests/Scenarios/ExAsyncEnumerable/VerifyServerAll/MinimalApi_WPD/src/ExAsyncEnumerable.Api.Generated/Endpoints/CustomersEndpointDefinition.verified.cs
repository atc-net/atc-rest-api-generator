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
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public sealed class CustomersEndpointDefinition : IEndpointDefinition
{
    internal const string ApiRouteBase = "/api/v1/customers";

    public void DefineEndpoints(
        WebApplication app)
    {
        var customers = app
            .NewVersionedApi("Customers")
            .MapGroup(ApiRouteBase);

        customers
            .MapGet("/", GetCustomers)
            .WithName("GetCustomers")
            .WithSummary("Get customers.")
            .WithDescription("Get customer.")
            .AddEndpointFilter<ValidationFilter<GetCustomersParameters>>()
            .Produces<IAsyncEnumerable<PaginationResult<Customer>>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    internal async Task<IResult> GetCustomers(
        [FromServices] IGetCustomersHandler handler,
        [AsParameters] GetCustomersParameters parameters,
        CancellationToken cancellationToken)
        => GetCustomersResult.ToIResult(
            await handler.ExecuteAsync(
                parameters,
                cancellationToken));
}