﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExAllResponseTypes.Api.Generated.Endpoints;

/// <summary>
/// Endpoint definitions.
/// </summary>
[ApiController]
[Route("/api/v1/example")]
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public sealed class ExampleController : ControllerBase
{
    /// <summary>
    /// Description: Example endpoint.
    /// Operation: GetExample.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status100Continue)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status101SwitchingProtocols)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status102Processing)]
    [ProducesResponseType(typeof(ProblemDetails), 103)]
    [ProducesResponseType(typeof(ExampleModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status203NonAuthoritative)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status205ResetContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status206PartialContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status207MultiStatus)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status208AlreadyReported)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status226IMUsed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status300MultipleChoices)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status301MovedPermanently)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status303SeeOther)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status304NotModified)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status305UseProxy)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status306SwitchProxy)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status307TemporaryRedirect)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status308PermanentRedirect)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status407ProxyAuthenticationRequired)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status411LengthRequired)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status413RequestEntityTooLarge)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status414RequestUriTooLong)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status416RequestedRangeNotSatisfiable)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status417ExpectationFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status421MisdirectedRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status423Locked)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status424FailedDependency)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status426UpgradeRequired)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status428PreconditionRequired)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status431RequestHeaderFieldsTooLarge)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status451UnavailableForLegalReasons)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status501NotImplemented)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status505HttpVersionNotsupported)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status506VariantAlsoNegotiates)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status507InsufficientStorage)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status508LoopDetected)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status510NotExtended)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status511NetworkAuthenticationRequired)]
    public async Task<ActionResult> GetExample(
        GetExampleParameters parameters,
        [FromServices] IGetExampleHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(parameters, cancellationToken);
}