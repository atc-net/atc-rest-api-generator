namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Client;

public static class ContentGeneratorClientEndpointParametersFactory
{
    public static ContentGeneratorClientEndpointParameters Create(
        bool useProblemDetailsAsDefaultResponseBody,
        string projectName,
        string apiGroupName,
        string @namespace,
        OpenApiPathItem openApiPath,
        OperationType httpMethod,
        OpenApiOperation openApiOperation,
        string httpClientName,
        string urlPath)
    {
        ArgumentNullException.ThrowIfNull(openApiPath);
        ArgumentNullException.ThrowIfNull(openApiOperation);

        var parameters = new List<ContentGeneratorClientEndpointParametersParameters>();

        AppendParameters(parameters, openApiPath.Parameters);
        AppendParameters(parameters, openApiOperation.Parameters);
        AppendParametersFromBody(parameters, openApiOperation.RequestBody);

        var operationName = openApiOperation.GetOperationName();
        var successResponseStatusCodes = openApiOperation.Responses
            .GetHttpStatusCodes()
            .Where(x => x.IsSuccessful())
            .ToList();

        OpenApiSchema? modelSchema = null;
        string? successResponseName = null;
        HttpStatusCode? successResponseStatusCode = null;

        if (successResponseStatusCodes.Count == 1)
        {
            successResponseStatusCode = successResponseStatusCodes[0];

            if (successResponseStatusCode is not HttpStatusCode.Accepted or HttpStatusCode.NoContent)
            {
                modelSchema = openApiOperation.Responses.GetSchemaForStatusCode(successResponseStatusCode.Value);
                successResponseName = GetSuccessResponseName(projectName, apiGroupName, modelSchema);
            }
        }

        var useListForDataType = modelSchema?.IsTypeArray() ?? false;

        var errorResponses = GetErrorResponses(
            useProblemDetailsAsDefaultResponseBody,
            openApiOperation,
            parameters.Any());

        if (openApiPath.HasParameters() ||
            openApiOperation.HasParametersOrRequestBody())
        {
            return new ContentGeneratorClientEndpointParameters(
                Namespace: @namespace,
                HttpMethod: httpMethod.ToString(),
                OperationName: operationName,
                DocumentationTagsForClass: openApiOperation.ExtractDocumentationTagsForEndpoint(),
                HttpClientName: httpClientName,
                UrlPath: urlPath,
                EndpointName: $"{operationName}{ContentGeneratorConstants.Endpoint}",
                InterfaceName: $"I{operationName}{ContentGeneratorConstants.Endpoint}",
                ResultName: $"{operationName}{ContentGeneratorConstants.EndpointResult}",
                ParameterName: $"{operationName}{ContentGeneratorConstants.Parameters}",
                SuccessResponseName: successResponseName,
                SuccessResponseStatusCode: successResponseStatusCode,
                UseListForModel: useListForDataType,
                ErrorResponses: errorResponses,
                parameters);
        }

        return new ContentGeneratorClientEndpointParameters(
            Namespace: @namespace,
            HttpMethod: httpMethod.ToString(),
            OperationName: operationName,
            DocumentationTagsForClass: openApiOperation.ExtractDocumentationTagsForEndpoint(),
            HttpClientName: httpClientName,
            UrlPath: urlPath,
            EndpointName: $"{operationName}{ContentGeneratorConstants.Endpoint}",
            InterfaceName: $"I{operationName}{ContentGeneratorConstants.Endpoint}",
            ResultName: $"{operationName}{ContentGeneratorConstants.EndpointResult}",
            ParameterName: null,
            SuccessResponseName: successResponseName,
            SuccessResponseStatusCode: successResponseStatusCode,
            UseListForModel: useListForDataType,
            ErrorResponses: errorResponses,
            Parameters: null);
    }

    private static void AppendParameters(
        ICollection<ContentGeneratorClientEndpointParametersParameters> parameters,
        IEnumerable<OpenApiParameter> openApiParameters)
    {
        foreach (var openApiParameter in openApiParameters)
        {
            if (parameters.FirstOrDefault(x => x.Name == openApiParameter.Name) is null)
            {
                parameters.Add(new ContentGeneratorClientEndpointParametersParameters(
                    openApiParameter.Name,
                    openApiParameter.Name.EnsureValidFormattedPropertyName(),
                    ConvertToParameterLocationType(openApiParameter.In),
                    openApiParameter.Schema.IsTypeArray()));
            }
        }
    }

    private static void AppendParametersFromBody(
        ICollection<ContentGeneratorClientEndpointParametersParameters> parameters,
        OpenApiRequestBody? requestBody)
    {
        var requestSchema = requestBody?.Content?.GetSchemaByFirstMediaType();

        if (requestSchema is null)
        {
            return;
        }

        parameters.Add(new ContentGeneratorClientEndpointParametersParameters(
            string.Empty,
            ContentGeneratorConstants.Request,
            requestSchema.GetParameterLocationType(),
            requestSchema.IsTypeArray()));
    }

    private static ParameterLocationType ConvertToParameterLocationType(
        ParameterLocation? openApiParameterLocation)
        => openApiParameterLocation switch
        {
            ParameterLocation.Query => ParameterLocationType.Query,
            ParameterLocation.Header => ParameterLocationType.Header,
            ParameterLocation.Path => ParameterLocationType.Route,
            ParameterLocation.Cookie => ParameterLocationType.Cookie,
            null => ParameterLocationType.None,
            _ => throw new SwitchCaseDefaultException(openApiParameterLocation),
        };

    private static string GetSuccessResponseName(
        string projectName,
        string apiGroupName,
        OpenApiSchema? modelSchema)
    {
        var modelName = "string";
        if (modelSchema is null)
        {
            return modelName;
        }

        var tmpModelName = modelSchema.GetModelName();
        if (string.IsNullOrEmpty(tmpModelName))
        {
            if (!modelSchema.IsTypeCustomPagination())
            {
                return modelSchema.GetDataType();
            }

            tmpModelName = modelSchema.GetCustomPaginationGenericTypeWithItemType(projectName, apiGroupName, isClient: true);
            return string.IsNullOrEmpty(tmpModelName)
                ? modelName
                : tmpModelName;
        }

        modelName = OpenApiDocumentSchemaModelNameResolver.EnsureModelNameWithNamespaceIfNeeded(
            projectName,
            apiGroupName,
            tmpModelName,
            isShared: false,
            isClient: true);

        return modelName;
    }

    private static List<ContentGeneratorClientEndpointErrorResponsesParameters> GetErrorResponses(
        bool useProblemDetailsAsDefaultResponseBody,
        OpenApiOperation openApiOperation,
        bool hasParameters)
    {
        var httpStatusCodes = openApiOperation.Responses
            .GetHttpStatusCodes()
            .Where(x => x.IsClientOrServerError() || x.IsRedirect())
            .ToList();

        if (hasParameters &&
            !httpStatusCodes.Contains(HttpStatusCode.BadRequest))
        {
            httpStatusCodes.Add(HttpStatusCode.BadRequest);
        }

        if (!httpStatusCodes.Contains(HttpStatusCode.Unauthorized))
        {
            httpStatusCodes.Add(HttpStatusCode.Unauthorized);
        }

        if (!httpStatusCodes.Contains(HttpStatusCode.Forbidden))
        {
            httpStatusCodes.Add(HttpStatusCode.Forbidden);
        }

        if (!httpStatusCodes.Contains(HttpStatusCode.InternalServerError))
        {
            httpStatusCodes.Add(HttpStatusCode.InternalServerError);
        }

        var errorResponses = new List<ContentGeneratorClientEndpointErrorResponsesParameters>();
        foreach (var httpStatusCode in httpStatusCodes.OrderBy(x => x))
        {
            if (httpStatusCode == HttpStatusCode.BadRequest)
            {
                errorResponses.Add(
                    new ContentGeneratorClientEndpointErrorResponsesParameters(
                        "ValidationProblemDetails",
                        httpStatusCode));
            }
            else if (httpStatusCode == HttpStatusCode.NotModified)
            {
                errorResponses.Add(
                    new ContentGeneratorClientEndpointErrorResponsesParameters(
                        string.Empty,
                        httpStatusCode));
            }
            else
            {
                if (useProblemDetailsAsDefaultResponseBody)
                {
                    errorResponses.Add(
                        new ContentGeneratorClientEndpointErrorResponsesParameters(
                            "ProblemDetails",
                            httpStatusCode));
                }
                else
                {
                    if (httpStatusCode == HttpStatusCode.InternalServerError)
                    {
                        errorResponses.Add(
                            new ContentGeneratorClientEndpointErrorResponsesParameters(
                                "string",
                                httpStatusCode));
                    }
                    else
                    {
                        errorResponses.Add(
                            new ContentGeneratorClientEndpointErrorResponsesParameters(
                                string.Empty,
                                httpStatusCode));
                    }
                }
            }
        }

        return errorResponses;
    }
}