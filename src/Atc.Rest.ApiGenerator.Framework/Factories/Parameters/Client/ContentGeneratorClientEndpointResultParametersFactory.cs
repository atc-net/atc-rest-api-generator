namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Client;

public static class ContentGeneratorClientEndpointResultParametersFactory
{
    public static ContentGeneratorClientEndpointResultParameters Create(
        bool useProblemDetailsAsDefaultResponseBody,
        string projectName,
        string apiGroupName,
        string @namespace,
        bool useProblemDetailsAsDefaultBody,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation)
    {
        ArgumentNullException.ThrowIfNull(openApiPath);
        ArgumentNullException.ThrowIfNull(openApiOperation);

        var parameters = new List<ContentGeneratorClientEndpointResultParametersParameters>();

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

        if (openApiPath.HasParameters() ||
            openApiOperation.HasParametersOrRequestBody())
        {
            return new ContentGeneratorClientEndpointResultParameters(
                Namespace: @namespace,
                OperationName: operationName,
                DocumentationTagsForClass: openApiOperation.ExtractDocumentationTagsForEndpointResult(),
                EndpointResultName: $"{operationName}{ContentGeneratorConstants.EndpointResult}",
                EndpointResultInterfaceName: $"I{operationName}{ContentGeneratorConstants.EndpointResult}",
                InheritClassName: ContentGeneratorConstants.EndpointResponse,
                SuccessResponseName: successResponseName,
                UseProblemDetailsAsDefaultBody: useProblemDetailsAsDefaultBody,
                UseListForModel: useListForDataType,
                ErrorResponses: errorResponses,
                parameters);
        }

        return new ContentGeneratorClientEndpointResultParameters(
            Namespace: @namespace,
            OperationName: operationName,
            DocumentationTagsForClass: openApiOperation.ExtractDocumentationTagsForEndpointResult(),
            EndpointResultName: $"{operationName}{ContentGeneratorConstants.EndpointResult}",
            EndpointResultInterfaceName: $"I{operationName}{ContentGeneratorConstants.EndpointResult}",
            InheritClassName: ContentGeneratorConstants.EndpointResponse,
            SuccessResponseName: successResponseName,
            UseProblemDetailsAsDefaultBody: useProblemDetailsAsDefaultBody,
            UseListForModel: useListForDataType,
            ErrorResponses: errorResponses,
            Parameters: null);
    }

    private static void AppendParameters(
        ICollection<ContentGeneratorClientEndpointResultParametersParameters> parameters,
        IEnumerable<OpenApiParameter> openApiParameters)
    {
        foreach (var openApiParameter in openApiParameters)
        {
            if (parameters.FirstOrDefault(x => x.Name == openApiParameter.Name) is null)
            {
                parameters.Add(new ContentGeneratorClientEndpointResultParametersParameters(
                    openApiParameter.Name,
                    openApiParameter.Name.EnsureFirstCharacterToUpper(),
                    ConvertToParameterLocationType(openApiParameter.In)));
            }
        }
    }

    private static void AppendParametersFromBody(
        ICollection<ContentGeneratorClientEndpointResultParametersParameters> parameters,
        OpenApiRequestBody? requestBody)
    {
        var requestSchema = requestBody?.Content?.GetSchemaByFirstMediaType();

        if (requestSchema is null)
        {
            return;
        }

        parameters.Add(new ContentGeneratorClientEndpointResultParametersParameters(
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
            if (modelSchema.IsTypeCustomPagination())
            {
                var customPaginationSchema = modelSchema.GetCustomPaginationSchema();
                var customPaginationItemsSchema = modelSchema.GetCustomPaginationItemsSchema();
                if (customPaginationSchema is null ||
                    customPaginationItemsSchema is null)
                {
                    return modelName;
                }

                var modelTypeName = customPaginationItemsSchema.IsSimpleDataType()
                    ? customPaginationItemsSchema.GetDataType()
                    : customPaginationItemsSchema.GetModelName();
                var customPaginationTypeName = customPaginationSchema.GetModelName();
                modelName = $"{customPaginationTypeName}<{modelTypeName}>";
            }
            else
            {
                modelName = modelSchema.GetDataType();
            }
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

    private static List<ContentGeneratorClientEndpointResultErrorResponsesParameters> GetErrorResponses(
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

        var errorResponses = new List<ContentGeneratorClientEndpointResultErrorResponsesParameters>();
        foreach (var httpStatusCode in httpStatusCodes.OrderBy(x => x))
        {
            if (httpStatusCode == HttpStatusCode.BadRequest)
            {
                errorResponses.Add(
                    new ContentGeneratorClientEndpointResultErrorResponsesParameters(
                        "ValidationProblemDetails",
                        httpStatusCode));
            }
            else
            {
                if (useProblemDetailsAsDefaultResponseBody)
                {
                    errorResponses.Add(
                        new ContentGeneratorClientEndpointResultErrorResponsesParameters(
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
                            new ContentGeneratorClientEndpointResultErrorResponsesParameters(
                                string.Empty,
                                httpStatusCode));
                    }
                    else
                    {
                        errorResponses.Add(
                            new ContentGeneratorClientEndpointResultErrorResponsesParameters(
                                "string",
                                httpStatusCode));
                    }
                }
            }
        }

        return errorResponses;
    }
}