﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExAllResponseTypes.Api.Generated.Contracts.Example;

/// <summary>
/// Results for operation request.
/// Description: Example endpoint.
/// Operation: GetExample.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetExampleResult : ResultBase
{
    private GetExampleResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 100 - Continue response.
    /// </summary>
    public static GetExampleResult Continue(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.Continue, message));

    /// <summary>
    /// 101 - SwitchingProtocols response.
    /// </summary>
    public static GetExampleResult SwitchingProtocols(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.SwitchingProtocols, message));

    /// <summary>
    /// 102 - Processing response.
    /// </summary>
    public static GetExampleResult Processing(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.Processing, message));

    /// <summary>
    /// 103 - EarlyHints response.
    /// </summary>
    public static GetExampleResult EarlyHints()
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.EarlyHints));

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static GetExampleResult Ok(ExampleModel response)
        => new GetExampleResult(new OkObjectResult(response));

    /// <summary>
    /// 201 - Created response.
    /// </summary>
    public static GetExampleResult Created(string? uri = null)
        => new GetExampleResult(ResultFactory.CreateContentResult(HttpStatusCode.Created, uri));

    /// <summary>
    /// 202 - Accepted response.
    /// </summary>
    public static GetExampleResult Accepted(string? uri = null)
        => new GetExampleResult(ResultFactory.CreateContentResult(HttpStatusCode.Accepted, uri));

    /// <summary>
    /// 203 - NonAuthoritativeInformation response.
    /// </summary>
    public static GetExampleResult NonAuthoritativeInformation(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.NonAuthoritativeInformation, message));

    /// <summary>
    /// 204 - NoContent response.
    /// </summary>
    public static GetExampleResult NoContent(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.NoContent, message));

    /// <summary>
    /// 205 - ResetContent response.
    /// </summary>
    public static GetExampleResult ResetContent(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.ResetContent, message));

    /// <summary>
    /// 206 - PartialContent response.
    /// </summary>
    public static GetExampleResult PartialContent(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.PartialContent, message));

    /// <summary>
    /// 207 - MultiStatus response.
    /// </summary>
    public static GetExampleResult MultiStatus(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.MultiStatus, message));

    /// <summary>
    /// 208 - AlreadyReported response.
    /// </summary>
    public static GetExampleResult AlreadyReported(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.AlreadyReported, message));

    /// <summary>
    /// 226 - ImUsed response.
    /// </summary>
    public static GetExampleResult ImUsed(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.IMUsed, message));

    /// <summary>
    /// 300 - MultipleChoices response.
    /// </summary>
    public static GetExampleResult MultipleChoices(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.MultipleChoices, message));

    /// <summary>
    /// 301 - MovedPermanently response.
    /// </summary>
    public static GetExampleResult MovedPermanently(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.MovedPermanently, message));

    /// <summary>
    /// 302 - Found response.
    /// </summary>
    public static GetExampleResult Found(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.Found, message));

    /// <summary>
    /// 303 - SeeOther response.
    /// </summary>
    public static GetExampleResult SeeOther(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.SeeOther, message));

    /// <summary>
    /// 304 - NotModified response.
    /// </summary>
    public static GetExampleResult NotModified(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.NotModified, message));

    /// <summary>
    /// 305 - UseProxy response.
    /// </summary>
    public static GetExampleResult UseProxy(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.UseProxy, message));

    /// <summary>
    /// 306 - Unused response.
    /// </summary>
    public static GetExampleResult Unused(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.Unused, message));

    /// <summary>
    /// 307 - RedirectKeepVerb response.
    /// </summary>
    public static GetExampleResult RedirectKeepVerb(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.RedirectKeepVerb, message));

    /// <summary>
    /// 308 - PermanentRedirect response.
    /// </summary>
    public static GetExampleResult PermanentRedirect(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.PermanentRedirect, message));

    /// <summary>
    /// 400 - BadRequest response.
    /// </summary>
    public static GetExampleResult BadRequest(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithValidationProblemDetails(HttpStatusCode.BadRequest, message));

    /// <summary>
    /// 401 - Unauthorized response.
    /// </summary>
    public static GetExampleResult Unauthorized(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.Unauthorized, message));

    /// <summary>
    /// 402 - PaymentRequired response.
    /// </summary>
    public static GetExampleResult PaymentRequired(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.PaymentRequired, message));

    /// <summary>
    /// 403 - Forbidden response.
    /// </summary>
    public static GetExampleResult Forbidden(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.Forbidden, message));

    /// <summary>
    /// 404 - NotFound response.
    /// </summary>
    public static GetExampleResult NotFound(string? message = null)
        => new GetExampleResult(new NotFoundObjectResult(message));

    /// <summary>
    /// 405 - MethodNotAllowed response.
    /// </summary>
    public static GetExampleResult MethodNotAllowed(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.MethodNotAllowed, message));

    /// <summary>
    /// 406 - NotAcceptable response.
    /// </summary>
    public static GetExampleResult NotAcceptable(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.NotAcceptable, message));

    /// <summary>
    /// 407 - ProxyAuthenticationRequired response.
    /// </summary>
    public static GetExampleResult ProxyAuthenticationRequired(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.ProxyAuthenticationRequired, message));

    /// <summary>
    /// 408 - RequestTimeout response.
    /// </summary>
    public static GetExampleResult RequestTimeout(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.RequestTimeout, message));

    /// <summary>
    /// 409 - Conflict response.
    /// </summary>
    public static GetExampleResult Conflict(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.Conflict, message));

    /// <summary>
    /// 410 - Gone response.
    /// </summary>
    public static GetExampleResult Gone(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.Gone, message));

    /// <summary>
    /// 411 - LengthRequired response.
    /// </summary>
    public static GetExampleResult LengthRequired(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.LengthRequired, message));

    /// <summary>
    /// 412 - PreconditionFailed response.
    /// </summary>
    public static GetExampleResult PreconditionFailed(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.PreconditionFailed, message));

    /// <summary>
    /// 413 - RequestEntityTooLarge response.
    /// </summary>
    public static GetExampleResult RequestEntityTooLarge(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.RequestEntityTooLarge, message));

    /// <summary>
    /// 414 - RequestUriTooLong response.
    /// </summary>
    public static GetExampleResult RequestUriTooLong(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.RequestUriTooLong, message));

    /// <summary>
    /// 415 - UnsupportedMediaType response.
    /// </summary>
    public static GetExampleResult UnsupportedMediaType(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.UnsupportedMediaType, message));

    /// <summary>
    /// 416 - RequestedRangeNotSatisfiable response.
    /// </summary>
    public static GetExampleResult RequestedRangeNotSatisfiable(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.RequestedRangeNotSatisfiable, message));

    /// <summary>
    /// 417 - ExpectationFailed response.
    /// </summary>
    public static GetExampleResult ExpectationFailed(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.ExpectationFailed, message));

    /// <summary>
    /// 421 - MisdirectedRequest response.
    /// </summary>
    public static GetExampleResult MisdirectedRequest(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.MisdirectedRequest, message));

    /// <summary>
    /// 422 - UnprocessableEntity response.
    /// </summary>
    public static GetExampleResult UnprocessableEntity(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.UnprocessableEntity, message));

    /// <summary>
    /// 423 - Locked response.
    /// </summary>
    public static GetExampleResult Locked(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.Locked, message));

    /// <summary>
    /// 424 - FailedDependency response.
    /// </summary>
    public static GetExampleResult FailedDependency(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.FailedDependency, message));

    /// <summary>
    /// 426 - UpgradeRequired response.
    /// </summary>
    public static GetExampleResult UpgradeRequired(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.UpgradeRequired, message));

    /// <summary>
    /// 428 - PreconditionRequired response.
    /// </summary>
    public static GetExampleResult PreconditionRequired(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.PreconditionRequired, message));

    /// <summary>
    /// 429 - TooManyRequests response.
    /// </summary>
    public static GetExampleResult TooManyRequests(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.TooManyRequests, message));

    /// <summary>
    /// 431 - RequestHeaderFieldsTooLarge response.
    /// </summary>
    public static GetExampleResult RequestHeaderFieldsTooLarge(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.RequestHeaderFieldsTooLarge, message));

    /// <summary>
    /// 451 - UnavailableForLegalReasons response.
    /// </summary>
    public static GetExampleResult UnavailableForLegalReasons(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.UnavailableForLegalReasons, message));

    /// <summary>
    /// 500 - InternalServerError response.
    /// </summary>
    public static GetExampleResult InternalServerError(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.InternalServerError, message));

    /// <summary>
    /// 501 - NotImplemented response.
    /// </summary>
    public static GetExampleResult NotImplemented(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.NotImplemented, message));

    /// <summary>
    /// 502 - BadGateway response.
    /// </summary>
    public static GetExampleResult BadGateway(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.BadGateway, message));

    /// <summary>
    /// 503 - ServiceUnavailable response.
    /// </summary>
    public static GetExampleResult ServiceUnavailable(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.ServiceUnavailable, message));

    /// <summary>
    /// 504 - GatewayTimeout response.
    /// </summary>
    public static GetExampleResult GatewayTimeout(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.GatewayTimeout, message));

    /// <summary>
    /// 505 - HttpVersionNotSupported response.
    /// </summary>
    public static GetExampleResult HttpVersionNotSupported(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.HttpVersionNotSupported, message));

    /// <summary>
    /// 506 - VariantAlsoNegotiates response.
    /// </summary>
    public static GetExampleResult VariantAlsoNegotiates(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.VariantAlsoNegotiates, message));

    /// <summary>
    /// 507 - InsufficientStorage response.
    /// </summary>
    public static GetExampleResult InsufficientStorage(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.InsufficientStorage, message));

    /// <summary>
    /// 508 - LoopDetected response.
    /// </summary>
    public static GetExampleResult LoopDetected(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.LoopDetected, message));

    /// <summary>
    /// 510 - NotExtended response.
    /// </summary>
    public static GetExampleResult NotExtended(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.NotExtended, message));

    /// <summary>
    /// 511 - NetworkAuthenticationRequired response.
    /// </summary>
    public static GetExampleResult NetworkAuthenticationRequired(string? message = null)
        => new GetExampleResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.NetworkAuthenticationRequired, message));
}