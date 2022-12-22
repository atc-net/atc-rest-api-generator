namespace Atc.Rest.ApiGenerator.Framework.Contracts.Models;

public record CodeDocumentationTags(
    string Summary,
    IList<(string Name, string Description)>? Parameters,
    string? Remark,
    string? Example,
    IList<(string ExceptionType, string Description)>? Exception,
    string? Return);