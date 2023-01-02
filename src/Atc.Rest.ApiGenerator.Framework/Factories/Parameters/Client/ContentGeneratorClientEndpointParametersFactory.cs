namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Client;

public static class ContentGeneratorClientEndpointParametersFactory
{
    public static ContentGeneratorClientEndpointParameters Create(
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
        if (openApiPath.HasParameters() ||
            openApiOperation.HasParametersOrRequestBody())
        {
            return new ContentGeneratorClientEndpointParameters(
                @namespace,
                httpMethod.ToString(),
                operationName,
                openApiOperation.ExtractDocumentationTagsForEndpoint(),
                httpClientName,
                urlPath,
                EndpointName: $"{operationName}{ContentGeneratorConstants.Endpoint}",
                InterfaceName: $"I{operationName}{ContentGeneratorConstants.Endpoint}",
                ResultName: $"{operationName}{ContentGeneratorConstants.EndpointResult}",
                ParameterName: $"{operationName}{ContentGeneratorConstants.Parameters}",
                parameters);
        }

        return new ContentGeneratorClientEndpointParameters(
            @namespace,
            httpMethod.ToString(),
            operationName,
            openApiOperation.ExtractDocumentationTagsForEndpoint(),
            httpClientName,
            urlPath,
            EndpointName: $"{operationName}{ContentGeneratorConstants.Endpoint}",
            InterfaceName: $"I{operationName}{ContentGeneratorConstants.Endpoint}",
            ResultName: $"{operationName}{ContentGeneratorConstants.EndpointResult}",
            ParameterName: null,
            Parameters: null);
    }

    private static void AppendParameters(
        ICollection<ContentGeneratorClientEndpointParametersParameters> parameters,
        IEnumerable<OpenApiParameter> openApiParameters)
    {
        foreach (var openApiParameter in openApiParameters)
        {
            var useListForDataType = openApiParameter.Schema.IsTypeArray();

            var dataType = useListForDataType
                ? openApiParameter.Schema.Items.GetDataType()
                : openApiParameter.Schema.GetDataType();

            var isSimpleType = useListForDataType
                ? openApiParameter.Schema.Items.IsSimpleDataType() || openApiParameter.Schema.Items.IsSchemaEnumOrPropertyEnum() || openApiParameter.Schema.Items.IsFormatTypeBinary()
                : openApiParameter.Schema.IsSimpleDataType() || openApiParameter.Schema.IsSchemaEnumOrPropertyEnum() || openApiParameter.Schema.IsFormatTypeBinary();

            if (parameters.FirstOrDefault(x => x.Name == openApiParameter.Name) is null)
            {
                parameters.Add(new ContentGeneratorClientEndpointParametersParameters(
                    openApiParameter.Name,
                    openApiParameter.Name.EnsureFirstCharacterToUpper(),
                    openApiParameter.ExtractDocumentationTags(),
                    ConvertToParameterLocationType(openApiParameter.In),
                    dataType,
                    isSimpleType,
                    useListForDataType,
                    GetIsNullable(openApiParameter, useListForDataType),
                    openApiParameter.Required,
                    GetAdditionalValidationAttributes(openApiParameter),
                    openApiParameter.Schema.GetDefaultValueAsString()));
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

        var isFormatTypeOfBinary = requestSchema.IsFormatTypeBinary();
        var isItemsOfFormatTypeBinary = requestSchema.HasItemsWithFormatTypeBinary();

        var requestBodyType = "string?";
        if (requestSchema.Reference is not null)
        {
            requestBodyType = requestSchema.Reference.Id.EnsureFirstCharacterToUpper();
        }
        else if (isFormatTypeOfBinary)
        {
            requestBodyType = "IFormFile";
        }
        else if (isItemsOfFormatTypeBinary)
        {
            requestBodyType = "IFormFile";
        }
        else if (requestSchema.Items is not null)
        {
            requestBodyType = requestSchema.Items.Reference.Id.EnsureFirstCharacterToUpper();
        }

        parameters.Add(new ContentGeneratorClientEndpointParametersParameters(
            string.Empty,
            ContentGeneratorConstants.Request,
            requestSchema.ExtractDocumentationTags(),
            requestSchema.GetParameterLocationType(),
            requestBodyType,
            IsSimpleType: false,
            UseListForDataType: requestSchema.IsTypeArray(),
            IsNullable: false,
            IsRequired: true,
            AdditionalValidationAttributes: new List<ValidationAttribute>(),
            DefaultValueInitializer: null));
    }

    private static bool GetIsNullable(
        OpenApiParameter openApiParameter,
        bool useListForDataType)
    {
        var isNullable = openApiParameter.Schema.Nullable;
        isNullable = isNullable switch
        {
            true when useListForDataType => false,
            false when openApiParameter.Schema.IsSchemaEnumOrPropertyEnum() => true,
            _ => isNullable,
        };

        return isNullable;
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

    private static IList<ValidationAttribute> GetAdditionalValidationAttributes(
        OpenApiParameter openApiParameter)
    {
        var validationAttributeExtractor = new ValidationAttributeExtractor();
        return validationAttributeExtractor.Extract(openApiParameter.Schema);
    }
}