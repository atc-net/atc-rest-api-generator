//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.Api.Generated.Contracts.Accounts;

/// <summary>
/// Results for operation request.
/// Description: Update name of account.
/// Operation: UpdateAccountName.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UpdateAccountNameResult : ResultBase
{
    private UpdateAccountNameResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static UpdateAccountNameResult Ok(string? message = null)
        => new UpdateAccountNameResult(new OkObjectResult(message));

    /// <summary>
    /// Performs an implicit conversion from UpdateAccountNameResult to ActionResult.
    /// </summary>
    public static implicit operator UpdateAccountNameResult(string response)
        => Ok(response);
}
