﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Users;

/// <summary>
/// Results for operation request.
/// Description: Get all users.
/// Operation: GetUsers.
/// </summary>
[GeneratedCode("ApiGenerator", "2.0.259.29354")]
public class GetUsersResult : ResultBase
{
    private GetUsersResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static GetUsersResult Ok(IEnumerable<User> response)
        => new GetUsersResult(new OkObjectResult(response ?? Enumerable.Empty<User>()));

    /// <summary>
    /// 409 - Conflict response.
    /// </summary>
    public static GetUsersResult Conflict(object? error = null)
        => new GetUsersResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.Conflict, error));

    /// <summary>
    /// Performs an implicit conversion from GetUsersResult to ActionResult.
    /// </summary>
    public static implicit operator GetUsersResult(List<User> response)
        => Ok(response);
}