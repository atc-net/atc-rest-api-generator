﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Structure1.Api.Generated.Users.MyContracts;

/// <summary>
/// Results for operation request.
/// Description: Delete user by id.
/// Operation: DeleteUserById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class DeleteUserByIdResult
{
    private DeleteUserByIdResult(IResult result)
    {
        Result = result;
    }

    public IResult Result { get; }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static DeleteUserByIdResult Ok(string? message = null)
        => new(TypedResults.Ok(message));

    /// <summary>
    /// 404 - NotFound response.
    /// </summary>
    public static DeleteUserByIdResult NotFound(string? message = null)
        => new(TypedResults.NotFound(message));

    /// <summary>
    /// 409 - Conflict response.
    /// </summary>
    public static DeleteUserByIdResult Conflict(string? message = null)
        => new(TypedResults.Conflict(message));

    /// <summary>
    /// Performs an implicit conversion from DeleteUserByIdResult to IResult.
    /// </summary>
    public static IResult ToIResult(DeleteUserByIdResult result)
        => result.Result;
}