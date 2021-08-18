﻿using System.CodeDom.Compiler;
using System.Threading;
using System.Threading.Tasks;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.1.154.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Files
{
    /// <summary>
    /// Domain Interface for RequestHandler.
    /// Description: Upload multi files as form data.
    /// Operation: UploadMultiFilesAsFormData.
    /// Area: Files.
    /// </summary>
    [GeneratedCode("ApiGenerator", "1.1.154.0")]
    public interface IUploadMultiFilesAsFormDataHandler
    {
        /// <summary>
        /// Execute method.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<UploadMultiFilesAsFormDataResult> ExecuteAsync(UploadMultiFilesAsFormDataParameters parameters, CancellationToken cancellationToken = default);
    }
}