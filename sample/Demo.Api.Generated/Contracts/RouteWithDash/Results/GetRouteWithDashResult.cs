﻿using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Atc.Rest.Results;
using Microsoft.AspNetCore.Mvc;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.0.216.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.RouteWithDash
{
    /// <summary>
    /// Results for operation request.
    /// Description: Your GET endpoint.
    /// Operation: GetRouteWithDash.
    /// Area: RouteWithDash.
    /// </summary>
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Should not throw ArgumentNullExceptions from implicit operators.")]
    [GeneratedCode("ApiGenerator", "1.0.216.0")]
    public class GetRouteWithDashResult : ResultBase
    {
        private GetRouteWithDashResult(ActionResult result) : base(result) { }

        /// <summary>
        /// 200 - Ok response.
        /// </summary>
        public static GetRouteWithDashResult Ok(string? message = null) => new GetRouteWithDashResult(ResultFactory.CreateContentResult(HttpStatusCode.OK, message));

        /// <summary>
        /// Performs an implicit conversion from GetRouteWithDashResult to ActionResult.
        /// </summary>
        public static implicit operator GetRouteWithDashResult(string x) => Ok(x);
    }
}