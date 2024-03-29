﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Files;

/// <summary>
/// Domain Interface for RequestHandler.
/// Description: Upload a file as FormData.
/// Operation: UploadSingleObjectWithFileAsFormData.
/// </summary>
[GeneratedCode("ApiGenerator", "2.0.259.29354")]
public interface IUploadSingleObjectWithFileAsFormDataHandler
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<UploadSingleObjectWithFileAsFormDataResult> ExecuteAsync(
        UploadSingleObjectWithFileAsFormDataParameters parameters,
        CancellationToken cancellationToken = default);
}