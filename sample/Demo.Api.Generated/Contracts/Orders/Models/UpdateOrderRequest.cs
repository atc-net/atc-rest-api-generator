﻿using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.0.181.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Orders
{
    /// <summary>
    /// Request to update an order.
    /// </summary>
    [GeneratedCode("ApiGenerator", "1.0.181.0")]
    public class UpdateOrderRequest
    {
        /// <summary>
        /// Undefined description.
        /// </summary>
        /// <remarks>
        /// Email validation being enforced.
        /// </remarks>
        [Required]
        [EmailAddress]
        public string MyEmail { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        public override string ToString()
        {
            return $"{nameof(MyEmail)}: {MyEmail}";
        }
    }
}