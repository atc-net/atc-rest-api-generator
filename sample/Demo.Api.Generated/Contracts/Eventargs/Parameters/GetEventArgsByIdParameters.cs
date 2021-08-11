﻿using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.1.124.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Eventargs
{
    /// <summary>
    /// Parameters for operation request.
    /// Description: Get EventArgs By Id.
    /// Operation: GetEventArgsById.
    /// Area: Eventargs.
    /// </summary>
    [GeneratedCode("ApiGenerator", "1.1.124.0")]
    public class GetEventArgsByIdParameters
    {
        /// <summary>
        /// The id of the eventArgs.
        /// </summary>
        [FromRoute(Name = "id")]
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}";
        }
    }
}