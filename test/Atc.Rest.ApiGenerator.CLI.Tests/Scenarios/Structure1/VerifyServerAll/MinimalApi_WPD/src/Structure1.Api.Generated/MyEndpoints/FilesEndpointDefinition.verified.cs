﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Structure1.Api.Generated.MyEndpoints;

/// <summary>
/// Endpoint definitions.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public sealed class FilesEndpointDefinition : IEndpointDefinition
{
    internal const string ApiRouteBase = "/api/v1/files";

    public void DefineEndpoints(
        WebApplication app)
    {
        var files = app
            .NewVersionedApi("Files")
            .MapGroup(ApiRouteBase);

        files
            .MapPost("form-data/multiFile", UploadMultiFilesAsFormData)
            .WithName("UploadMultiFilesAsFormData")
            .WithSummary("Upload multi files as form data.")
            .WithDescription("Upload multi files as form data.")
            .AddEndpointFilter<ValidationFilter<UploadMultiFilesAsFormDataParameters>>()
            .Produces<string?>()
            .ProducesValidationProblem();

        files
            .MapPost("form-data/singleFile", UploadSingleFileAsFormData)
            .WithName("UploadSingleFileAsFormData")
            .WithSummary("Upload a file as OctetStream.")
            .WithDescription("Upload a file as OctetStream.")
            .AddEndpointFilter<ValidationFilter<UploadSingleFileAsFormDataParameters>>()
            .Produces<string?>()
            .ProducesValidationProblem();

        files
            .MapPost("form-data/singleObject", UploadSingleObjectWithFileAsFormData)
            .WithName("UploadSingleObjectWithFileAsFormData")
            .WithSummary("Upload a file as FormData.")
            .WithDescription("Upload a file as FormData.")
            .AddEndpointFilter<ValidationFilter<UploadSingleObjectWithFileAsFormDataParameters>>()
            .Produces<string?>()
            .ProducesValidationProblem();

        files
            .MapPost("form-data/singleObjectMultiFile", UploadSingleObjectWithFilesAsFormData)
            .WithName("UploadSingleObjectWithFilesAsFormData")
            .WithSummary("Upload files as FormData.")
            .WithDescription("Upload files as FormData.")
            .AddEndpointFilter<ValidationFilter<UploadSingleObjectWithFilesAsFormDataParameters>>()
            .Produces<string?>()
            .ProducesValidationProblem();

        files
            .MapGet("{id}", GetFileById)
            .WithName("GetFileById")
            .WithSummary("Get File By Id.")
            .WithDescription("Get File By Id.")
            .AddEndpointFilter<ValidationFilter<GetFileByIdParameters>>()
            .Produces<string?>()
            .ProducesValidationProblem()
            .Produces<string?>(StatusCodes.Status404NotFound);
    }

    internal async Task<IResult> UploadMultiFilesAsFormData(
        [FromServices] IUploadMultiFilesAsFormDataHandler handler,
        [AsParameters] UploadMultiFilesAsFormDataParameters parameters,
        CancellationToken cancellationToken)
        => UploadMultiFilesAsFormDataResult.ToIResult(
            await handler.ExecuteAsync(
                parameters,
                cancellationToken));

    internal async Task<IResult> UploadSingleFileAsFormData(
        [FromServices] IUploadSingleFileAsFormDataHandler handler,
        [AsParameters] UploadSingleFileAsFormDataParameters parameters,
        CancellationToken cancellationToken)
        => UploadSingleFileAsFormDataResult.ToIResult(
            await handler.ExecuteAsync(
                parameters,
                cancellationToken));

    internal async Task<IResult> UploadSingleObjectWithFileAsFormData(
        [FromServices] IUploadSingleObjectWithFileAsFormDataHandler handler,
        [AsParameters] UploadSingleObjectWithFileAsFormDataParameters parameters,
        CancellationToken cancellationToken)
        => UploadSingleObjectWithFileAsFormDataResult.ToIResult(
            await handler.ExecuteAsync(
                parameters,
                cancellationToken));

    internal async Task<IResult> UploadSingleObjectWithFilesAsFormData(
        [FromServices] IUploadSingleObjectWithFilesAsFormDataHandler handler,
        [AsParameters] UploadSingleObjectWithFilesAsFormDataParameters parameters,
        CancellationToken cancellationToken)
        => UploadSingleObjectWithFilesAsFormDataResult.ToIResult(
            await handler.ExecuteAsync(
                parameters,
                cancellationToken));

    internal async Task<IResult> GetFileById(
        [FromServices] IGetFileByIdHandler handler,
        [AsParameters] GetFileByIdParameters parameters,
        CancellationToken cancellationToken)
        => GetFileByIdResult.ToIResult(
            await handler.ExecuteAsync(
                parameters,
                cancellationToken));
}