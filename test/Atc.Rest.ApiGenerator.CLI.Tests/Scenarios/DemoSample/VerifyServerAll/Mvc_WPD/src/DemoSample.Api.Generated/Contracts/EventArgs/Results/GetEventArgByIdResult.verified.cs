﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.Api.Generated.Contracts.EventArgs;

/// <summary>
/// Results for operation request.
/// Description: Get EventArgs By Id.
/// Operation: GetEventArgById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetEventArgByIdResult : ResultBase
{
    private GetEventArgByIdResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static GetEventArgByIdResult Ok(EventArgs response)
        => new GetEventArgByIdResult(new OkObjectResult(response));

    /// <summary>
    /// 404 - NotFound response.
    /// </summary>
    public static GetEventArgByIdResult NotFound(string? message = null)
        => new GetEventArgByIdResult(new NotFoundObjectResult(message));

    /// <summary>
    /// Performs an implicit conversion from GetEventArgByIdResult to ActionResult.
    /// </summary>
    public static implicit operator GetEventArgByIdResult(EventArgs response)
        => Ok(response);
}