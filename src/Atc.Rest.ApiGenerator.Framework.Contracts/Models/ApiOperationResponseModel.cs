namespace Atc.Rest.ApiGenerator.Framework.Contracts.Models;

public record ApiOperationResponseModel(
    HttpStatusCode StatusCode,
    string OperationName,
    string? GroupName,
    string? MediaType,
    string? CollectionDataType,
    string? DataType,
    string? Description);