﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExAsyncEnumerable.Api.Generated.Contracts.Customers;

/// <summary>
/// Results for operation request.
/// Description: Get customers.
/// Operation: GetCustomers.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetCustomersResult
{
    private GetCustomersResult(IResult result)
    {
        Result = result;
    }

    public IResult Result { get; }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static GetCustomersResult Ok(IAsyncEnumerable<PaginationResult<Customer>> result)
        => new(TypedResults.Ok(result));

    /// <summary>
    /// Performs an implicit conversion from GetCustomersResult to IResult.
    /// </summary>
    public static IResult ToIResult(GetCustomersResult result)
        => result.Result;
}