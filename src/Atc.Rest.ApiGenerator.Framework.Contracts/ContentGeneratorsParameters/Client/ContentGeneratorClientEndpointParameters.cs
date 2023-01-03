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
    string? SuccessResponseName,
    bool UseListForModel,
    IList<ContentGeneratorClientEndpointErrorResponsesParameters> ErrorResponses,
    IList<ContentGeneratorClientEndpointParametersParameters>? Parameters);

public record ContentGeneratorClientEndpointErrorResponsesParameters(
    string ResponseType,
    HttpStatusCode StatusCode);

public record ContentGeneratorClientEndpointParametersParameters(
    string Name,
    string ParameterName,
    ParameterLocationType ParameterLocationType);