//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.ApiClient.Generated.Endpoints.Files;

/// <summary>
/// Client Endpoint result.
/// Description: Get File By Id.
/// Operation: GetFileById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetFileByIdEndpointResult : EndpointResponse, IGetFileByIdEndpointResult
{
    public GetFileByIdEndpointResult(EndpointResponse response)
        : base(response)
    {
    }

    public bool IsOk
        => StatusCode == HttpStatusCode.OK;

    public bool IsBadRequest
        => StatusCode == HttpStatusCode.BadRequest;

    public bool IsUnauthorized
        => StatusCode == HttpStatusCode.Unauthorized;

    public bool IsNotFound
        => StatusCode == HttpStatusCode.NotFound;

    public byte[] OkContent
        => IsOk && ContentObject is byte[] result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsOk property first.");

    public ValidationProblemDetails BadRequestContent
        => IsBadRequest && ContentObject is ValidationProblemDetails result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsBadRequest property first.");

    public ProblemDetails UnauthorizedContent
        => IsUnauthorized && ContentObject is ProblemDetails result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsUnauthorized property first.");

    public ProblemDetails NotFoundContent
        => IsNotFound && ContentObject is ProblemDetails result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsNotFound property first.");
}