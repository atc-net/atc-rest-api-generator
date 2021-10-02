﻿using System.CodeDom.Compiler;
using Atc.Rest.Results;
using Microsoft.AspNetCore.Mvc;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Api.Generated.Contracts.Accounts
{
    /// <summary>
    /// Results for operation request.
    /// Description: Set name of account.
    /// Operation: SetAccountName.
    /// Area: Accounts.
    /// </summary>
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    public class SetAccountNameResult : ResultBase
    {
        private SetAccountNameResult(ActionResult result) : base(result) { }

        /// <summary>
        /// 200 - Ok response.
        /// </summary>
        public static SetAccountNameResult Ok(string? message = null) => new SetAccountNameResult(new OkObjectResult(message));

        /// <summary>
        /// Performs an implicit conversion from SetAccountNameResult to ActionResult.
        /// </summary>
        public static implicit operator SetAccountNameResult(string response) => Ok(response);
    }
}