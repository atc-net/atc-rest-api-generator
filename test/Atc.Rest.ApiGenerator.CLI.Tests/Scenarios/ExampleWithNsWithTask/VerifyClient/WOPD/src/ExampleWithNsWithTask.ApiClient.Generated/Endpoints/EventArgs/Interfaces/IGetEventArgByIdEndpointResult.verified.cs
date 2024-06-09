//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExampleWithNsWithTask.ApiClient.Generated.Endpoints.EventArgs.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Get EventArgs By Id.
/// Operation: GetEventArgById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetEventArgByIdEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    bool IsNotFound { get; }

    EventArgs OkContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }

    string? NotFoundContent { get; }
}
