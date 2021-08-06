﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Atc.Rest.Results;
using Microsoft.AspNetCore.Mvc;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.1.124.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Tasks
{
    /// <summary>
    /// Results for operation request.
    /// Description: Returns tasks.
    /// Operation: GetTasks.
    /// Area: Tasks.
    /// </summary>
    [GeneratedCode("ApiGenerator", "1.1.124.0")]
    public class GetTasksResult : ResultBase
    {
        private GetTasksResult(ActionResult result) : base(result) { }

        /// <summary>
        /// 200 - Ok response.
        /// </summary>
        public static GetTasksResult Ok(IEnumerable<Task> response) => new GetTasksResult(new OkObjectResult(response ?? Enumerable.Empty<Task>()));

        /// <summary>
        /// Performs an implicit conversion from GetTasksResult to ActionResult.
        /// </summary>
        public static implicit operator GetTasksResult(List<Task> response) => Ok(response);
    }
}