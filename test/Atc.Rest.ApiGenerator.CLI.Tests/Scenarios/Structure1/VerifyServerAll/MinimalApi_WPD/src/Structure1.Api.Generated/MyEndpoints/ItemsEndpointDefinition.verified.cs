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
public sealed class ItemsEndpointDefinition : IEndpointDefinition
{
    internal const string ApiRouteBase = "/api/v1/items";

    public void DefineEndpoints(
        WebApplication app)
    {
        var items = app
            .NewVersionedApi("Items")
            .MapGroup(ApiRouteBase);

        items
            .MapPost("/", CreateItem)
            .WithName("CreateItem")
            .WithSummary("Create a new item.")
            .WithDescription("Create a new item.")
            .AddEndpointFilter<ValidationFilter<CreateItemParameters>>()
            .Produces<string?>()
            .ProducesValidationProblem();

        items
            .MapPut("{id}", UpdateItem)
            .WithName("UpdateItem")
            .WithSummary("Updates an item.")
            .WithDescription("Updates an item.")
            .AddEndpointFilter<ValidationFilter<UpdateItemParameters>>()
            .Produces<Guid>()
            .ProducesValidationProblem();
    }

    internal async Task<IResult> CreateItem(
        [FromServices] ICreateItemHandler handler,
        [AsParameters] CreateItemParameters parameters,
        CancellationToken cancellationToken)
        => CreateItemResult.ToIResult(
            await handler.ExecuteAsync(
                parameters,
                cancellationToken));

    internal async Task<IResult> UpdateItem(
        [FromServices] IUpdateItemHandler handler,
        [AsParameters] UpdateItemParameters parameters,
        CancellationToken cancellationToken)
        => UpdateItemResult.ToIResult(
            await handler.ExecuteAsync(
                parameters,
                cancellationToken));
}