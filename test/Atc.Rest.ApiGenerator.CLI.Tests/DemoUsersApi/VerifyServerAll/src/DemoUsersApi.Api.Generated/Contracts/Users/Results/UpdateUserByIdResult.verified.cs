﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoUsersApi.Api.Generated.Contracts.Users;

/// <summary>
/// Results for operation request.
/// Description: Update user by id.
/// Operation: UpdateUserById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UpdateUserByIdResult : ResultBase
{
    private UpdateUserByIdResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static UpdateUserByIdResult Ok(string? message = null)
        => new UpdateUserByIdResult(new OkObjectResult(message));

    /// <summary>
    /// 400 - BadRequest response.
    /// </summary>
    public static UpdateUserByIdResult BadRequest(string? message = null)
        => new UpdateUserByIdResult(new BadRequestObjectResult(message));

    /// <summary>
    /// 404 - NotFound response.
    /// </summary>
    public static UpdateUserByIdResult NotFound(string? message = null)
        => new UpdateUserByIdResult(new NotFoundObjectResult(message));

    /// <summary>
    /// 409 - Conflict response.
    /// </summary>
    public static UpdateUserByIdResult Conflict(string? message = null)
        => new UpdateUserByIdResult(new ConflictObjectResult(message));

    /// <summary>
    /// Performs an implicit conversion from UpdateUserByIdResult to ActionResult.
    /// </summary>
    public static implicit operator UpdateUserByIdResult(string response)
        => Ok(response);
}