﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace CodeStructure1.ApiClient.Generated.Files.MyContracts;

/// <summary>
/// Parameters for operation request.
/// Description: Upload a file as FormData.
/// Operation: UploadSingleObjectWithFileAsFormData.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UploadSingleObjectWithFileAsFormDataParameters
{
    /// <summary>
    /// FileAsFormDataRequest.
    /// </summary>
    [Required]
    public FileAsFormDataRequest Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Request)}: ({Request})";
}