//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExNsWithTask.ApiClient.Generated.Endpoints.Tasks;

/// <summary>
/// Client Endpoint result.
/// Description: Returns tasks.
/// Operation: GetTasks.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetTasksEndpointResult : EndpointResponse, IGetTasksEndpointResult
{
    public GetTasksEndpointResult(EndpointResponse response)
        : base(response)
    {
    }

    public bool IsOk
        => StatusCode == HttpStatusCode.OK;

    public bool IsUnauthorized
        => StatusCode == HttpStatusCode.Unauthorized;

    public IEnumerable<Task> OkContent
        => IsOk && ContentObject is IEnumerable<Task> result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsOk property first.");

    public string? UnauthorizedContent
        => IsUnauthorized && ContentObject is string result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsUnauthorized property first.");
}