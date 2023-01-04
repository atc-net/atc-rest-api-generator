namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Client;

public record ContentGeneratorClientEndpointResultParameters(
    string Namespace,
    string OperationName,
    CodeDocumentationTags DocumentationTagsForClass,
    string EndpointResultName,
    string InheritClassName,
    string? SuccessResponseName,
    bool UseListForModel,
    IList<ContentGeneratorClientEndpointResultErrorResponsesParameters> ErrorResponses,
    IList<ContentGeneratorClientEndpointResultParametersParameters>? Parameters);

public record ContentGeneratorClientEndpointResultErrorResponsesParameters(
    string ResponseType,
    HttpStatusCode StatusCode);

public record ContentGeneratorClientEndpointResultParametersParameters(
    string Name,
    string ParameterName,
    ParameterLocationType ParameterLocationType);