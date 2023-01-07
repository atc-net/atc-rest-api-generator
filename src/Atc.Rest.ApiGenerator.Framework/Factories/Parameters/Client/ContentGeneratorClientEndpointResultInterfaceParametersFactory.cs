namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Client;

public static class ContentGeneratorClientEndpointResultInterfaceParametersFactory
{
    public static ContentGeneratorClientEndpointResultInterfaceParameters Create(
        bool useProblemDetailsAsDefaultResponseBody,
        string projectName,
        string apiGroupName,
        string @namespace,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation)
    {
        ArgumentNullException.ThrowIfNull(openApiPath);
        ArgumentNullException.ThrowIfNull(openApiOperation);

        var parameters = new List<ContentGeneratorClientEndpointResultInterfaceParametersParameters>();

        AppendParameters(parameters, openApiPath.Parameters);
        AppendParameters(parameters, openApiOperation.Parameters);
        AppendParametersFromBody(parameters, openApiOperation.RequestBody);

        var operationName = openApiOperation.GetOperationName();
        var modelSchema = openApiOperation.Responses.GetSchemaForStatusCode(HttpStatusCode.OK);
        var successResponseName = GetSuccessResponseName(projectName, apiGroupName, modelSchema);
        var useListForDataType = modelSchema?.IsTypeArray() ?? false;

        var errorResponses = GetErrorResponses(
            useProblemDetailsAsDefaultResponseBody,
            openApiOperation,
            parameters.Any());

        return new ContentGeneratorClientEndpointResultInterfaceParameters(
            Namespace: @namespace,
            OperationName: operationName,
            DocumentationTagsForClass: openApiOperation.ExtractDocumentationTagsForEndpointResultInterface(),
            InterfaceName: $"I{operationName}{ContentGeneratorConstants.EndpointResult}",
            InheritInterfaceName: "IEndpointResponse",
            SuccessResponseName: successResponseName,
            UseProblemDetailsAsDefaultBody: useProblemDetailsAsDefaultResponseBody,
            UseListForModel: useListForDataType,
            ErrorResponses: errorResponses);
    }

    private static void AppendParameters(
        ICollection<ContentGeneratorClientEndpointResultInterfaceParametersParameters> parameters,
        IEnumerable<OpenApiParameter> openApiParameters)
    {
        foreach (var openApiParameter in openApiParameters)
        {
            if (parameters.FirstOrDefault(x => x.Name == openApiParameter.Name) is null)
            {
                parameters.Add(new ContentGeneratorClientEndpointResultInterfaceParametersParameters(
                    openApiParameter.Name,
                    openApiParameter.Name.EnsureFirstCharacterToUpper(),
                    ConvertToParameterLocationType(openApiParameter.In)));
            }
        }
    }

    private static void AppendParametersFromBody(
        ICollection<ContentGeneratorClientEndpointResultInterfaceParametersParameters> parameters,
        OpenApiRequestBody? requestBody)
    {
        var requestSchema = requestBody?.Content?.GetSchemaByFirstMediaType();

        if (requestSchema is null)
        {
            return;
        }

        parameters.Add(new ContentGeneratorClientEndpointResultInterfaceParametersParameters(
            string.Empty,
            ContentGeneratorConstants.Request,
            requestSchema.GetParameterLocationType()));
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
            modelName = modelSchema.GetDataType();
        }
        else
        {
            modelName = OpenApiDocumentSchemaModelNameResolver.EnsureModelNameWithNamespaceIfNeeded(
                projectName,
                apiGroupName,
                tmpModelName,
                isShared: false,
                isClient: true);
        }

        return modelName;
    }

    private static List<ContentGeneratorClientEndpointResultInterfaceErrorResponsesParameters> GetErrorResponses(
        bool useProblemDetailsAsDefaultResponseBody,
        OpenApiOperation openApiOperation,
        bool hasParameters)
    {
        var httpStatusCodes = openApiOperation.Responses
            .GetHttpStatusCodes()
            .Where(x => x.IsClientOrServerError())
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

        var errorResponses = new List<ContentGeneratorClientEndpointResultInterfaceErrorResponsesParameters>();
        foreach (var httpStatusCode in httpStatusCodes.OrderBy(x => x))
        {
            if (httpStatusCode == HttpStatusCode.BadRequest)
            {
                errorResponses.Add(
                    new ContentGeneratorClientEndpointResultInterfaceErrorResponsesParameters(
                        "ValidationProblemDetails",
                        httpStatusCode));
            }
            else
            {
                if (useProblemDetailsAsDefaultResponseBody)
                {
                    errorResponses.Add(
                        new ContentGeneratorClientEndpointResultInterfaceErrorResponsesParameters(
                            "ProblemDetails",
                            httpStatusCode));
                }
                else
                {
                    if (httpStatusCode == HttpStatusCode.Accepted ||
                        httpStatusCode == HttpStatusCode.NoContent ||
                        httpStatusCode == HttpStatusCode.NotModified ||
                        httpStatusCode == HttpStatusCode.Unauthorized ||
                        httpStatusCode == HttpStatusCode.Forbidden)
                    {
                        errorResponses.Add(
                            new ContentGeneratorClientEndpointResultInterfaceErrorResponsesParameters(
                                string.Empty,
                                httpStatusCode));
                    }
                    else
                    {
                        errorResponses.Add(
                            new ContentGeneratorClientEndpointResultInterfaceErrorResponsesParameters(
                                "string",
                                httpStatusCode));
                    }
                }
            }
        }

        return errorResponses;
    }
}