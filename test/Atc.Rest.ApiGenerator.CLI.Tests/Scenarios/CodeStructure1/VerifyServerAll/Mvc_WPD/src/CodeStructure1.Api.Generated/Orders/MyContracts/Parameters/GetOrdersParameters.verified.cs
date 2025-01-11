﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace CodeStructure1.Api.Generated.Orders.MyContracts;

/// <summary>
/// Parameters for operation request.
/// Description: Get orders.
/// Operation: GetOrders.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetOrdersParameters
{
    /// <summary>
    /// The numbers of items to return.
    /// </summary>
    [FromQuery]
    [Required]
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// The number of items to skip before starting to collect the result set.
    /// </summary>
    [FromQuery]
    [Range(0, 2147483647)]
    public int PageIndex { get; set; } = 0;

    /// <summary>
    /// The query string.
    /// </summary>
    [FromQuery]
    public string? QueryString { get; set; }

    /// <summary>
    /// The query array of string.
    /// </summary>
    [FromQuery]
    public List<string> QueryStringArray { get; set; } = new List<string>();

    /// <summary>
    /// The continuation token.
    /// </summary>
    [FromQuery]
    public string? ContinuationToken { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(PageSize)}: {PageSize}, {nameof(PageIndex)}: {PageIndex}, {nameof(QueryString)}: {QueryString}, {nameof(QueryStringArray)}.Count: {QueryStringArray?.Count ?? 0}, {nameof(ContinuationToken)}: {ContinuationToken}";
}