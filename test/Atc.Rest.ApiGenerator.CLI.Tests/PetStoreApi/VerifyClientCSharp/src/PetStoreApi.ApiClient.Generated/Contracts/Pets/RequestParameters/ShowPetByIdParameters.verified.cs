//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace PetStoreApi.ApiClient.Generated.Contracts.Pets;

/// <summary>
/// Parameters for operation request.
/// Description: Info for a specific pet.
/// Operation: ShowPetById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class ShowPetByIdParameters
{
    /// <summary>
    /// The id of the pet to retrieve.
    /// </summary>
    [Required]
    public string PetId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(PetId)}: {PetId}";
}