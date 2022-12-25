namespace Atc.Rest.ApiGenerator.Framework.ContentGenerators;

public interface ICodeDocumentationTagsGenerator
{
    bool ShouldGenerateTags(
        CodeDocumentationTags codeDocumentationTags);

    string GenerateTags(
        ushort indentSpaces,
        CodeDocumentationTags codeDocumentationTags);

    string GenerateEndpointClassSummary(
        ushort indentSpaces);

    string GenerateEndpointMethodSummary(
        ushort indentSpaces,
        CodeDocumentationTags codeDocumentationTags);

    string GenerateResultMethodSummary(
        ushort indentSpaces,
        HttpStatusCode httpStatusCode);

    string GenerateHandlerMethodTags(
        ushort indentSpaces,
        string? parameterName);
}