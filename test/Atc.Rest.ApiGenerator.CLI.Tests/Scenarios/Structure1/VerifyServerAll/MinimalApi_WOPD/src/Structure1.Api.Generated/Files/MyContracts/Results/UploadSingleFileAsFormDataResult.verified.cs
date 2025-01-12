﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Structure1.Api.Generated.Files.MyContracts;

/// <summary>
/// Results for operation request.
/// Description: Upload a file as OctetStream.
/// Operation: UploadSingleFileAsFormData.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UploadSingleFileAsFormDataResult
{
    private UploadSingleFileAsFormDataResult(IResult result)
    {
        Result = result;
    }

    public IResult Result { get; }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static UploadSingleFileAsFormDataResult Ok(string? message = null)
        => new(TypedResults.Ok(message));

    /// <summary>
    /// Performs an implicit conversion from UploadSingleFileAsFormDataResult to IResult.
    /// </summary>
    public static IResult ToIResult(UploadSingleFileAsFormDataResult result)
        => result.Result;
}