﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace PetStore.ApiClient.Generated.Endpoints.Pets;

/// <summary>
/// Client Endpoint result.
/// Description: Create a pet.
/// Operation: CreatePets.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class CreatePetsEndpointResult : EndpointResponse, ICreatePetsEndpointResult
{
    public CreatePetsEndpointResult(EndpointResponse response)
        : base(response)
    {
    }

    public bool IsCreated
        => StatusCode == HttpStatusCode.Created;

    public ProblemDetails CreatedContent
        => IsCreated && ContentObject is ProblemDetails result
            ? result
            : throw new InvalidOperationException("Content is not the expected type - please use the IsCreated property first.");
}