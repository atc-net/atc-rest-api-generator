﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.1.154.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Files
{
    /// <summary>
    /// FilesAsFormDataRequest.
    /// </summary>
    [GeneratedCode("ApiGenerator", "1.1.154.0")]
    public class FilesAsFormDataRequest
    {
        /// <summary>
        /// A list of File(s).
        /// </summary>
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();

        /// <summary>
        /// Converts to string.
        /// </summary>
        public override string ToString()
        {
            return $"{nameof(Files)}.Count: {Files?.Count ?? 0}";
        }
    }
}