//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace PetStore.ApiClient.Generated.Endpoints.Pets.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: List all pets.
/// Operation: ListPets.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IListPetsEndpointResult : IEndpointResponse
{
    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    bool IsForbidden { get; }

    bool IsInternalServerError { get; }

    List<Pet> OkContent { get; }

    ValidationProblemDetails BadRequestContent { get; }

    string InternalServerErrorContent { get; }
}
