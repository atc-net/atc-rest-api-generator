﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExAsyncEnumerable.Api.Generated.Contracts.Customers;

/// <summary>
/// Parameters for operation request.
/// Description: Get customers.
/// Operation: GetCustomers.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetCustomersParameters
{
    /// <summary>
    /// The numbers of items to return.
    /// </summary>
    [FromQuery(Name = "pageSize")]
    [Required]
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// The number of items to skip before starting to collect the result set.
    /// </summary>
    [FromQuery(Name = "pageIndex")]
    [Range(0, 2147483647)]
    public int PageIndex { get; set; } = 0;

    [FromHeader(Name = "x-continuation")]
    public string? Continuation { get; set; }

    [FromQuery(Name = "filter")]
    public string? Filter { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(PageSize)}: {PageSize}, {nameof(PageIndex)}: {PageIndex}, {nameof(Continuation)}: {Continuation}, {nameof(Filter)}: {Filter}";
}