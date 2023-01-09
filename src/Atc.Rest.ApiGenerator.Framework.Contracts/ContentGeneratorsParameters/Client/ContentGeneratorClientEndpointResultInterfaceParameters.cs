namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Client;

public record ContentGeneratorClientEndpointResultInterfaceParameters(
    string Namespace,
    string OperationName,
    CodeDocumentationTags DocumentationTagsForClass,
    string InterfaceName,
    string InheritInterfaceName,
    string? SuccessResponseName,
    bool UseProblemDetailsAsDefaultBody,
    bool UseListForModel,
    IList<ContentGeneratorClientEndpointResultInterfaceErrorResponsesParameters> ErrorResponses);

public record ContentGeneratorClientEndpointResultInterfaceErrorResponsesParameters(
    string ResponseType,
    HttpStatusCode StatusCode);

public record ContentGeneratorClientEndpointResultInterfaceParametersParameters(
    string Name,
    string ParameterName,
    ParameterLocationType ParameterLocationType);