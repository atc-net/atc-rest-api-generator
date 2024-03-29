﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Files;

/// <summary>
/// Results for operation request.
/// Description: Upload a file as FormData.
/// Operation: UploadSingleObjectWithFileAsFormData.
/// </summary>
[GeneratedCode("ApiGenerator", "2.0.259.29354")]
public class UploadSingleObjectWithFileAsFormDataResult : ResultBase
{
    private UploadSingleObjectWithFileAsFormDataResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static UploadSingleObjectWithFileAsFormDataResult Ok(string? message = null)
        => new UploadSingleObjectWithFileAsFormDataResult(new OkObjectResult(message));

    /// <summary>
    /// 400 - BadRequest response.
    /// </summary>
    public static UploadSingleObjectWithFileAsFormDataResult BadRequest(string message)
        => new UploadSingleObjectWithFileAsFormDataResult(ResultFactory.CreateContentResultWithValidationProblemDetails(HttpStatusCode.BadRequest, message));

    /// <summary>
    /// Performs an implicit conversion from UploadSingleObjectWithFileAsFormDataResult to ActionResult.
    /// </summary>
    public static implicit operator UploadSingleObjectWithFileAsFormDataResult(string response)
        => Ok(response);
}