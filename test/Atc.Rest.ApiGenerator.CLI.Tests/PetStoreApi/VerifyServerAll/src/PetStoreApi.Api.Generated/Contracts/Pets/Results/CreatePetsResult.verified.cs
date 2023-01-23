﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace PetStoreApi.Api.Generated.Contracts.Pets;

/// <summary>
/// Results for operation request.
/// Description: Create a pet.
/// Operation: CreatePets.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class CreatePetsResult : ResultBase
{
    private CreatePetsResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 201 - Created response.
    /// </summary>
    public static CreatePetsResult Created()
        => new CreatePetsResult(new StatusCodeResult(StatusCodes.Status201Created));
}