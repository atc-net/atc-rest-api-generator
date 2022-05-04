﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Eventargs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.121.412.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Endpoints
{
    /// <summary>
    /// Endpoint definitions.
    /// Area: Eventargs.
    /// </summary>
    [ApiController]
    [Route("/api/v1/eventArgs")]
    [GeneratedCode("ApiGenerator", "2.0.121.412")]
    public class EventargsController : ControllerBase
    {
        /// <summary>
        /// Description: Get EventArgs List.
        /// Operation: GetEventArgs.
        /// Area: Eventargs.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Demo.Api.Generated.Contracts.Eventargs.EventArgs>), StatusCodes.Status200OK)]
        public Task<ActionResult> GetEventArgsAsync([FromServices] IGetEventArgsHandler handler, CancellationToken cancellationToken)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return InvokeGetEventArgsAsync(handler, cancellationToken);
        }

        /// <summary>
        /// Description: Get EventArgs By Id.
        /// Operation: GetEventArgById.
        /// Area: Eventargs.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Demo.Api.Generated.Contracts.Eventargs.EventArgs), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public Task<ActionResult> GetEventArgByIdAsync(GetEventArgByIdParameters parameters, [FromServices] IGetEventArgByIdHandler handler, CancellationToken cancellationToken)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return InvokeGetEventArgByIdAsync(parameters, handler, cancellationToken);
        }

        private static async Task<ActionResult> InvokeGetEventArgsAsync([FromServices] IGetEventArgsHandler handler, CancellationToken cancellationToken)
        {
            return await handler.ExecuteAsync(cancellationToken);
        }

        private static async Task<ActionResult> InvokeGetEventArgByIdAsync(GetEventArgByIdParameters parameters, IGetEventArgByIdHandler handler, CancellationToken cancellationToken)
        {
            return await handler.ExecuteAsync(parameters, cancellationToken);
        }
    }
}