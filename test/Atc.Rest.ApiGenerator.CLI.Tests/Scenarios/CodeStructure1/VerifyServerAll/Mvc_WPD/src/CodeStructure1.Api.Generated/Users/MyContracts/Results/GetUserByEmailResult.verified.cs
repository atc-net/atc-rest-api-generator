﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace CodeStructure1.Api.Generated.Users.MyContracts;

/// <summary>
/// Results for operation request.
/// Description: Get user by email.
/// Operation: GetUserByEmail.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetUserByEmailResult : ResultBase
{
    private GetUserByEmailResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static GetUserByEmailResult Ok(User response)
        => new GetUserByEmailResult(new OkObjectResult(response));

    /// <summary>
    /// 400 - BadRequest response.
    /// </summary>
    public static GetUserByEmailResult BadRequest(string? message = null)
        => new GetUserByEmailResult(ResultFactory.CreateContentResultWithValidationProblemDetails(HttpStatusCode.BadRequest, message));

    /// <summary>
    /// 404 - NotFound response.
    /// </summary>
    public static GetUserByEmailResult NotFound(string? message = null)
        => new GetUserByEmailResult(new NotFoundObjectResult(message));

    /// <summary>
    /// 409 - Conflict response.
    /// </summary>
    public static GetUserByEmailResult Conflict(string? message = null)
        => new GetUserByEmailResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.Conflict, message));

    /// <summary>
    /// Performs an implicit conversion from GetUserByEmailResult to ActionResult.
    /// </summary>
    public static implicit operator GetUserByEmailResult(User response)
        => Ok(response);
}