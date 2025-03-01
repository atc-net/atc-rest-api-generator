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
public sealed class EventArgsEndpointDefinition : IEndpointDefinition
{
    internal const string ApiRouteBase = "/api/v1/eventArgs";

    public void DefineEndpoints(
        WebApplication app)
    {
        var eventArgs = app
            .NewVersionedApi("EventArgs")
            .MapGroup(ApiRouteBase);

        eventArgs
            .MapGet("/", GetEventArgs)
            .WithName("GetEventArgs")
            .WithSummary("Get EventArgs List.")
            .WithDescription("Get EventArgs List.")
            .Produces<IEnumerable<EventArgs.MyContracts.EventArgs>>();

        eventArgs
            .MapGet("{id}", GetEventArgById)
            .WithName("GetEventArgById")
            .WithSummary("Get EventArgs By Id.")
            .WithDescription("Get EventArgs By Id.")
            .AddEndpointFilter<ValidationFilter<GetEventArgByIdParameters>>()
            .Produces<EventArgs.MyContracts.EventArgs>()
            .ProducesValidationProblem()
            .Produces<string?>(StatusCodes.Status404NotFound);
    }

    internal async Task<IResult> GetEventArgs(
        [FromServices] IGetEventArgsHandler handler,
        CancellationToken cancellationToken)
        => GetEventArgsResult.ToIResult(
            await handler.ExecuteAsync(
                cancellationToken));

    internal async Task<IResult> GetEventArgById(
        [FromServices] IGetEventArgByIdHandler handler,
        [AsParameters] GetEventArgByIdParameters parameters,
        CancellationToken cancellationToken)
        => GetEventArgByIdResult.ToIResult(
            await handler.ExecuteAsync(
                parameters,
                cancellationToken));
}