﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Files;

/// <summary>
/// Parameters for operation request.
/// Description: Upload files as FormData.
/// Operation: UploadSingleObjectWithFilesAsFormData.
/// </summary>
[GeneratedCode("ApiGenerator", "2.0.259.29354")]
public class UploadSingleObjectWithFilesAsFormDataParameters
{
    /// <summary>
    /// FilesAsFormDataRequest.
    /// </summary>
    [FromForm]
    [Required]
    public FilesAsFormDataRequest Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Request)}: ({Request})";
}