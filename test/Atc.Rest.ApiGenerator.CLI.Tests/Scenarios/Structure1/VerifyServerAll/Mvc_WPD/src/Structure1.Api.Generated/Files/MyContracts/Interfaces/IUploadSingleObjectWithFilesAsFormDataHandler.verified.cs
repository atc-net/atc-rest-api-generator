﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Structure1.Api.Generated.Files.MyContracts;

/// <summary>
/// Domain Interface for RequestHandler.
/// Description: Upload files as FormData.
/// Operation: UploadSingleObjectWithFilesAsFormData.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IUploadSingleObjectWithFilesAsFormDataHandler
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<UploadSingleObjectWithFilesAsFormDataResult> ExecuteAsync(
        UploadSingleObjectWithFilesAsFormDataParameters parameters,
        CancellationToken cancellationToken = default);
}