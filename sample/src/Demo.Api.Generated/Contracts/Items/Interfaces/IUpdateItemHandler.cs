﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Items;

/// <summary>
/// Domain Interface for RequestHandler.
/// Description: Updates an item.
/// Operation: UpdateItem.
/// </summary>
[GeneratedCode("ApiGenerator", "2.0.259.29354")]
public interface IUpdateItemHandler
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<UpdateItemResult> ExecuteAsync(
        UpdateItemParameters parameters,
        CancellationToken cancellationToken = default);
}