namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Client;

public record ContentGeneratorClientEndpointParameters(
    string Namespace,
    string HttpMethod,
    string OperationName,
    CodeDocumentationTags DocumentationTagsForClass,
    string HttpClientName,
    string UrlPath,
    string EndpointName,
    string InterfaceName,
    string ResultName,
    string? ParameterName,
    IList<ContentGeneratorClientEndpointParametersParameters>? Parameters);

public record ContentGeneratorClientEndpointParametersParameters(
    string Name,
    string ParameterName,
    CodeDocumentationTags DocumentationTags,
    ParameterLocationType ParameterLocationType,
    string DataType,
    bool IsSimpleType,
    bool UseListForDataType,
    bool IsNullable,
    bool IsRequired,
    IList<ValidationAttribute> AdditionalValidationAttributes,
    string? DefaultValueInitializer);