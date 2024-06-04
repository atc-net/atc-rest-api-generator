namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerResultParameters(
    string Namespace,
    string OperationName,
    CodeDocumentationTags DocumentationTags,
    string ResultName,
    IList<ContentGeneratorServerResultMethodParameters> MethodParameters,
    ContentGeneratorServerResultImplicitOperatorParameters? ImplicitOperatorParameters);

public record ContentGeneratorServerResultMethodParameters(
    CodeDocumentationTags DocumentationTags,
    ApiOperationResponseModel ResponseModel);

public record ContentGeneratorServerResultImplicitOperatorParameters(
    string? CollectionDataType,
    string? DataType);