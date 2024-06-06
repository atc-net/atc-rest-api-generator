//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.Api.Generated.Contracts.Items;

/// <summary>
/// Results for operation request.
/// Description: Updates an item.
/// Operation: UpdateItem.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UpdateItemResult : ResultBase
{
    private UpdateItemResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static UpdateItemResult Ok(Guid response)
        => new UpdateItemResult(new OkObjectResult(response));

    /// <summary>
    /// Performs an implicit conversion from UpdateItemResult to ActionResult.
    /// </summary>
    public static implicit operator UpdateItemResult(Guid response)
        => Ok(response);
}
