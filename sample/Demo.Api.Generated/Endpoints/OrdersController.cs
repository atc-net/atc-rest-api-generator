﻿using System;
using System.CodeDom.Compiler;
using System.Threading;
using System.Threading.Tasks;
using Atc.Rest.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.1.117.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Endpoints
{
    using Demo.Api.Generated.Contracts.Orders;

    /// <summary>
    /// Endpoint definitions.
    /// Area: Orders.
    /// </summary>
    [ApiController]
    [Route("/api/v1/orders")]
    [GeneratedCode("ApiGenerator", "1.1.117.0")]
    public class OrdersController : ControllerBase
    {
        /// <summary>
        /// Description: Get orders.
        /// Operation: GetOrders.
        /// Area: Orders.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Pagination<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public Task<ActionResult> GetOrdersAsync(GetOrdersParameters parameters, [FromServices] IGetOrdersHandler handler, CancellationToken cancellationToken)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return InvokeGetOrdersAsync(parameters, handler, cancellationToken);
        }

        /// <summary>
        /// Description: Get order by id.
        /// Operation: GetOrderById.
        /// Area: Orders.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public Task<ActionResult> GetOrderByIdAsync(GetOrderByIdParameters parameters, [FromServices] IGetOrderByIdHandler handler, CancellationToken cancellationToken)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return InvokeGetOrderByIdAsync(parameters, handler, cancellationToken);
        }

        /// <summary>
        /// Description: Update part of order by id.
        /// Operation: PatchOrdersId.
        /// Area: Orders.
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
        public Task<ActionResult> PatchOrdersIdAsync(PatchOrdersIdParameters parameters, [FromServices] IPatchOrdersIdHandler handler, CancellationToken cancellationToken)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return InvokePatchOrdersIdAsync(parameters, handler, cancellationToken);
        }

        private static async Task<ActionResult> InvokeGetOrdersAsync(GetOrdersParameters parameters, IGetOrdersHandler handler, CancellationToken cancellationToken)
        {
            return await handler.ExecuteAsync(parameters, cancellationToken);
        }

        private static async Task<ActionResult> InvokeGetOrderByIdAsync(GetOrderByIdParameters parameters, IGetOrderByIdHandler handler, CancellationToken cancellationToken)
        {
            return await handler.ExecuteAsync(parameters, cancellationToken);
        }

        private static async Task<ActionResult> InvokePatchOrdersIdAsync(PatchOrdersIdParameters parameters, IPatchOrdersIdHandler handler, CancellationToken cancellationToken)
        {
            return await handler.ExecuteAsync(parameters, cancellationToken);
        }
    }
}