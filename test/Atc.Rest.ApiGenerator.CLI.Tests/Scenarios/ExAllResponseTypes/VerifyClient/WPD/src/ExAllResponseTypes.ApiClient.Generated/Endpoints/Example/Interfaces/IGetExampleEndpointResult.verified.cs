﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExAllResponseTypes.ApiClient.Generated.Endpoints.Example.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Example endpoint.
/// Operation: GetExample.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetExampleEndpointResult : IEndpointResponse
{

    bool IsContinue { get; }

    bool IsSwitchingProtocols { get; }

    bool IsProcessing { get; }

    bool IsEarlyHints { get; }

    bool IsOk { get; }

    bool IsCreated { get; }

    bool IsAccepted { get; }

    bool IsNonAuthoritativeInformation { get; }

    bool IsNoContent { get; }

    bool IsResetContent { get; }

    bool IsPartialContent { get; }

    bool IsMultiStatus { get; }

    bool IsAlreadyReported { get; }

    bool IsImUsed { get; }

    bool IsMultipleChoices { get; }

    bool IsMovedPermanently { get; }

    bool IsFound { get; }

    bool IsSeeOther { get; }

    bool IsNotModified { get; }

    bool IsUseProxy { get; }

    bool IsUnused { get; }

    bool IsRedirectKeepVerb { get; }

    bool IsPermanentRedirect { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    bool IsPaymentRequired { get; }

    bool IsForbidden { get; }

    bool IsNotFound { get; }

    bool IsMethodNotAllowed { get; }

    bool IsNotAcceptable { get; }

    bool IsProxyAuthenticationRequired { get; }

    bool IsRequestTimeout { get; }

    bool IsConflict { get; }

    bool IsGone { get; }

    bool IsLengthRequired { get; }

    bool IsPreconditionFailed { get; }

    bool IsRequestEntityTooLarge { get; }

    bool IsRequestUriTooLong { get; }

    bool IsUnsupportedMediaType { get; }

    bool IsRequestedRangeNotSatisfiable { get; }

    bool IsExpectationFailed { get; }

    bool IsMisdirectedRequest { get; }

    bool IsUnprocessableEntity { get; }

    bool IsLocked { get; }

    bool IsFailedDependency { get; }

    bool IsUpgradeRequired { get; }

    bool IsPreconditionRequired { get; }

    bool IsTooManyRequests { get; }

    bool IsRequestHeaderFieldsTooLarge { get; }

    bool IsUnavailableForLegalReasons { get; }

    bool IsInternalServerError { get; }

    bool IsNotImplemented { get; }

    bool IsBadGateway { get; }

    bool IsServiceUnavailable { get; }

    bool IsGatewayTimeout { get; }

    bool IsHttpVersionNotSupported { get; }

    bool IsVariantAlsoNegotiates { get; }

    bool IsInsufficientStorage { get; }

    bool IsLoopDetected { get; }

    bool IsNotExtended { get; }

    bool IsNetworkAuthenticationRequired { get; }

    ProblemDetails ContinueContent { get; }

    ProblemDetails SwitchingProtocolsContent { get; }

    ProblemDetails ProcessingContent { get; }

    ProblemDetails EarlyHintsContent { get; }

    ExampleModel OkContent { get; }

    ProblemDetails CreatedContent { get; }

    ProblemDetails AcceptedContent { get; }

    ProblemDetails NonAuthoritativeInformationContent { get; }

    ProblemDetails NoContentContent { get; }

    ProblemDetails ResetContentContent { get; }

    ProblemDetails PartialContentContent { get; }

    ProblemDetails MultiStatusContent { get; }

    ProblemDetails AlreadyReportedContent { get; }

    ProblemDetails ImUsedContent { get; }

    ProblemDetails MultipleChoicesContent { get; }

    ProblemDetails MovedPermanentlyContent { get; }

    ProblemDetails FoundContent { get; }

    ProblemDetails SeeOtherContent { get; }

    ProblemDetails NotModifiedContent { get; }

    ProblemDetails UseProxyContent { get; }

    ProblemDetails UnusedContent { get; }

    ProblemDetails RedirectKeepVerbContent { get; }

    ProblemDetails PermanentRedirectContent { get; }

    ValidationProblemDetails BadRequestContent { get; }

    ProblemDetails UnauthorizedContent { get; }

    ProblemDetails PaymentRequiredContent { get; }

    ProblemDetails ForbiddenContent { get; }

    string? NotFoundContent { get; }

    ProblemDetails MethodNotAllowedContent { get; }

    ProblemDetails NotAcceptableContent { get; }

    ProblemDetails ProxyAuthenticationRequiredContent { get; }

    ProblemDetails RequestTimeoutContent { get; }

    ProblemDetails ConflictContent { get; }

    ProblemDetails GoneContent { get; }

    ProblemDetails LengthRequiredContent { get; }

    ProblemDetails PreconditionFailedContent { get; }

    ProblemDetails RequestEntityTooLargeContent { get; }

    ProblemDetails RequestUriTooLongContent { get; }

    ProblemDetails UnsupportedMediaTypeContent { get; }

    ProblemDetails RequestedRangeNotSatisfiableContent { get; }

    ProblemDetails ExpectationFailedContent { get; }

    ProblemDetails MisdirectedRequestContent { get; }

    ProblemDetails UnprocessableEntityContent { get; }

    ProblemDetails LockedContent { get; }

    ProblemDetails FailedDependencyContent { get; }

    ProblemDetails UpgradeRequiredContent { get; }

    ProblemDetails PreconditionRequiredContent { get; }

    ProblemDetails TooManyRequestsContent { get; }

    ProblemDetails RequestHeaderFieldsTooLargeContent { get; }

    ProblemDetails UnavailableForLegalReasonsContent { get; }

    ProblemDetails InternalServerErrorContent { get; }

    ProblemDetails NotImplementedContent { get; }

    ProblemDetails BadGatewayContent { get; }

    ProblemDetails ServiceUnavailableContent { get; }

    ProblemDetails GatewayTimeoutContent { get; }

    ProblemDetails HttpVersionNotSupportedContent { get; }

    ProblemDetails VariantAlsoNegotiatesContent { get; }

    ProblemDetails InsufficientStorageContent { get; }

    ProblemDetails LoopDetectedContent { get; }

    ProblemDetails NotExtendedContent { get; }

    ProblemDetails NetworkAuthenticationRequiredContent { get; }
}