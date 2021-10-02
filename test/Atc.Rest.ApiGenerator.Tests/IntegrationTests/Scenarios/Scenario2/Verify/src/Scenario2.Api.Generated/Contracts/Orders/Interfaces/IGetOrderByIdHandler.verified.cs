﻿using System.CodeDom.Compiler;
using System.Threading;
using System.Threading.Tasks;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Api.Generated.Contracts.Orders
{
    /// <summary>
    /// Domain Interface for RequestHandler.
    /// Description: Get order by id.
    /// Operation: GetOrderById.
    /// Area: Orders.
    /// </summary>
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    public interface IGetOrderByIdHandler
    {
        /// <summary>
        /// Execute method.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<GetOrderByIdResult> ExecuteAsync(GetOrderByIdParameters parameters, CancellationToken cancellationToken = default);
    }
}