namespace Atc.Rest.ApiGenerator.Framework.Contracts.Models;

public record CodeDocumentationTags(
    string Summary,
    List<string>? Parameters,
    string? Example,
    string? Exception,
    string? Return);