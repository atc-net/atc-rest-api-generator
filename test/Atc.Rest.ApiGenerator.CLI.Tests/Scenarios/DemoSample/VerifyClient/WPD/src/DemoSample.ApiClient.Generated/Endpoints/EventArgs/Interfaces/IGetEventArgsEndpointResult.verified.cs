//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.ApiClient.Generated.Endpoints.EventArgs.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Get EventArgs List.
/// Operation: GetEventArgs.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetEventArgsEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsUnauthorized { get; }

    IEnumerable<EventArgs> OkContent { get; }

    ProblemDetails UnauthorizedContent { get; }
}
