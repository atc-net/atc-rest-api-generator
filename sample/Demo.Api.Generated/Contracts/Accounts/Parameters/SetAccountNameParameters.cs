﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.0.181.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Accounts
{
    /// <summary>
    /// Parameters for operation request.
    /// Description: Set name of account.
    /// Operation: SetAccountName.
    /// Area: Accounts.
    /// </summary>
    [GeneratedCode("ApiGenerator", "1.0.181.0")]
    public class SetAccountNameParameters
    {
        /// <summary>
        /// The accountId.
        /// </summary>
        [FromRoute(Name = "accountId")]
        [Required]
        public Guid AccountId { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        public override string ToString()
        {
            return $"{nameof(AccountId)}: {AccountId}";
        }
    }
}