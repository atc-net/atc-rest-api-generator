﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Items;

/// <summary>
/// Domain Interface for RequestHandler.
/// Description: Create a new item.
/// Operation: CreateItem.
/// </summary>
[GeneratedCode("ApiGenerator", "2.0.259.29354")]
public interface ICreateItemHandler
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<CreateItemResult> ExecuteAsync(
        CreateItemParameters parameters,
        CancellationToken cancellationToken = default);
}