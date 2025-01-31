﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Structure1.Api.Generated.Files.MyContracts;

/// <summary>
/// Results for operation request.
/// Description: Upload multi files as form data.
/// Operation: UploadMultiFilesAsFormData.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UploadMultiFilesAsFormDataResult : ResultBase
{
    private UploadMultiFilesAsFormDataResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static UploadMultiFilesAsFormDataResult Ok(string? message = null)
        => new UploadMultiFilesAsFormDataResult(new OkObjectResult(message));

    /// <summary>
    /// Performs an implicit conversion from UploadMultiFilesAsFormDataResult to ActionResult.
    /// </summary>
    public static implicit operator UploadMultiFilesAsFormDataResult(string response)
        => Ok(response);
}