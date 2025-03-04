﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Structure1.Api.Generated.Users.MyContracts;

/// <summary>
/// Results for operation request.
/// Description: Get user by id.
/// Operation: GetUserById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetUserByIdResult
{
    private GetUserByIdResult(IResult result)
    {
        Result = result;
    }

    public IResult Result { get; }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static GetUserByIdResult Ok(User result)
        => new(TypedResults.Ok(result));

    /// <summary>
    /// 404 - NotFound response.
    /// </summary>
    public static GetUserByIdResult NotFound(string? message = null)
        => new(TypedResults.NotFound(message));

    /// <summary>
    /// 409 - Conflict response.
    /// </summary>
    public static GetUserByIdResult Conflict(string? message = null)
        => new(TypedResults.Conflict(message));

    /// <summary>
    /// Performs an implicit conversion from GetUserByIdResult to IResult.
    /// </summary>
    public static IResult ToIResult(GetUserByIdResult result)
        => result.Result;
}