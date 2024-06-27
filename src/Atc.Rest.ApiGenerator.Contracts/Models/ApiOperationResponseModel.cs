namespace Atc.Rest.ApiGenerator.Contracts.Models;

public record ApiOperationResponseModel(
    HttpStatusCode StatusCode,
    string OperationName,
    string? GroupName,
    string? MediaType,
    string? CollectionDataType,
    string? DataType,
    string? Description,
    string? Namespace);