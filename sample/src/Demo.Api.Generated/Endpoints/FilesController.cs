﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.220.5882.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Endpoints;

/// <summary>
/// Endpoint definitions.
/// </summary>
[ApiController]
[Route("/api/v1/files")]
[GeneratedCode("ApiGenerator", "2.0.220.5882")]
public class FilesController : ControllerBase
{
    /// <summary>
    /// Description: Get File By Id.
    /// Operation: GetFileById.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetFileById(
        GetFileByIdParameters parameters,
        [FromServices] IGetFileByIdHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(parameters, cancellationToken);

    /// <summary>
    /// Description: Upload multi files as form data.
    /// Operation: UploadMultiFilesAsFormData.
    /// </summary>
    [HttpPost("form-data/multiFile")]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UploadMultiFilesAsFormData(
        UploadMultiFilesAsFormDataParameters parameters,
        [FromServices] IUploadMultiFilesAsFormDataHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(parameters, cancellationToken);

    /// <summary>
    /// Description: Upload a file as OctetStream.
    /// Operation: UploadSingleFileAsFormData.
    /// </summary>
    [HttpPost("form-data/singleFile")]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UploadSingleFileAsFormData(
        UploadSingleFileAsFormDataParameters parameters,
        [FromServices] IUploadSingleFileAsFormDataHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(parameters, cancellationToken);

    /// <summary>
    /// Description: Upload a file as FormData.
    /// Operation: UploadSingleObjectWithFileAsFormData.
    /// </summary>
    [HttpPost("form-data/singleObject")]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UploadSingleObjectWithFileAsFormData(
        UploadSingleObjectWithFileAsFormDataParameters parameters,
        [FromServices] IUploadSingleObjectWithFileAsFormDataHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(parameters, cancellationToken);

    /// <summary>
    /// Description: Upload files as FormData.
    /// Operation: UploadSingleObjectWithFilesAsFormData.
    /// </summary>
    [HttpPost("form-data/singleObjectMultiFile")]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UploadSingleObjectWithFilesAsFormData(
        UploadSingleObjectWithFilesAsFormDataParameters parameters,
        [FromServices] IUploadSingleObjectWithFilesAsFormDataHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(parameters, cancellationToken);
}