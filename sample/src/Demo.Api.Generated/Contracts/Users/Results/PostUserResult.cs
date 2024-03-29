﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Users;

/// <summary>
/// Results for operation request.
/// Description: Create a new user.
/// Operation: PostUser.
/// </summary>
[GeneratedCode("ApiGenerator", "2.0.259.29354")]
public class PostUserResult : ResultBase
{
    private PostUserResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 201 - Created response.
    /// </summary>
    public static PostUserResult Created()
        => new PostUserResult(ResultFactory.CreateContentResult(HttpStatusCode.Created, null));

    /// <summary>
    /// 400 - BadRequest response.
    /// </summary>
    public static PostUserResult BadRequest(string message)
        => new PostUserResult(ResultFactory.CreateContentResultWithValidationProblemDetails(HttpStatusCode.BadRequest, message));

    /// <summary>
    /// 409 - Conflict response.
    /// </summary>
    public static PostUserResult Conflict(object? error = null)
        => new PostUserResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.Conflict, error));
}