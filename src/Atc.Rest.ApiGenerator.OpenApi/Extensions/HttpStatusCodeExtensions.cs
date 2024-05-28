namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class HttpStatusCodeExtensions
{
    public static bool HasMvcWellDefinedObjectResultClassForStatusCode(
        this HttpStatusCode httpStatusCode)
        => httpStatusCode is
            HttpStatusCode.BadRequest or
            HttpStatusCode.NotFound or
            HttpStatusCode.Conflict;

    public static bool HasMvcWellDefinedResultClassForStatusCode(
        this HttpStatusCode httpStatusCode)
        => httpStatusCode is
            HttpStatusCode.Accepted or
            HttpStatusCode.NoContent or
            HttpStatusCode.Unauthorized;

    public static bool IsMvcWellDefinedObjectResultClassForStatusCodeUsingNotNullMessage(
        this HttpStatusCode httpStatusCode)
        => httpStatusCode is
            HttpStatusCode.BadRequest;

    public static bool UseMvcWellDefinedStatusCodeResultClassForStatusCode(
        this HttpStatusCode httpStatusCode)
        => httpStatusCode is
            HttpStatusCode.Created or
            HttpStatusCode.BadRequest;

    public static bool UseMvcWellDefinedContentResultClassForStatusCode(
        this HttpStatusCode httpStatusCode)
        => httpStatusCode.IsServerError() ||
           httpStatusCode is
               HttpStatusCode.Forbidden or
               HttpStatusCode.MethodNotAllowed;

    public static bool UseMinimalApiWellDefinedProblemHttpResultClassForStatusCode(
        this HttpStatusCode httpStatusCode)
        => httpStatusCode.IsServerError() ||
           httpStatusCode is
               HttpStatusCode.MethodNotAllowed;
}