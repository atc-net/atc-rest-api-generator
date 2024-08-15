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
public class GetCustomersResult : ResultBase
{
    private GetCustomersResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static GetCustomersResult Ok(IAsyncEnumerable<PaginationResult<Customer>> response)
        => new GetCustomersResult(new OkObjectResult(response));

    /// <summary>
    /// Performs an implicit conversion from GetCustomersResult to ActionResult.
    /// </summary>
    public static implicit operator GetCustomersResult(PaginationResult<Customer> response)
        => Ok(AsyncEnumerableFactory.FromSingleItem(response));
}