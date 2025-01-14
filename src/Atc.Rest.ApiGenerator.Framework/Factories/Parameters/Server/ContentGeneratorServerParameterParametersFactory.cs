// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable MergeIntoPattern
namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Server;

public static class ContentGeneratorServerParameterParametersFactory
{
    public static ContentGeneratorServerParameterParameters CreateForClass(
        string @namespace,
        OpenApiOperation openApiOperation,
        IList<OpenApiParameter> globalPathParameters,
        string contractNamespaceWithoutApiGroupName)
    {
        ArgumentNullException.ThrowIfNull(openApiOperation);
        ArgumentNullException.ThrowIfNull(globalPathParameters);

        var operationName = openApiOperation.GetOperationName();

        var parameters = new List<ContentGeneratorServerParameterParametersProperty>();

        AppendParameters(parameters, globalPathParameters, contractNamespaceWithoutApiGroupName);
        AppendParameters(parameters, openApiOperation.Parameters, contractNamespaceWithoutApiGroupName);
        AppendParametersFromBody(parameters, openApiOperation.RequestBody);

        return new ContentGeneratorServerParameterParameters(
            @namespace,
            operationName,
            openApiOperation.ExtractDocumentationTagsForParameters(),
            DeclarationModifiers.PublicClass,
            ParameterName: $"{operationName}{ContentGeneratorConstants.Parameters}",
            parameters);
    }

    public static ContentGeneratorServerParameterParameters CreateForRecord(
        string @namespace,
        OpenApiOperation openApiOperation,
        IList<OpenApiParameter> globalPathParameters,
        string contractNamespaceWithoutApiGroupName)
    {
        ArgumentNullException.ThrowIfNull(openApiOperation);
        ArgumentNullException.ThrowIfNull(globalPathParameters);

        var operationName = openApiOperation.GetOperationName();

        var parameters = new List<ContentGeneratorServerParameterParametersProperty>();

        AppendParameters(parameters, globalPathParameters, contractNamespaceWithoutApiGroupName);
        AppendParameters(parameters, openApiOperation.Parameters, contractNamespaceWithoutApiGroupName);
        AppendParametersFromBody(parameters, openApiOperation.RequestBody);

        return new ContentGeneratorServerParameterParameters(
            @namespace,
            operationName,
            openApiOperation.ExtractDocumentationTagsForParameters(),
            DeclarationModifiers.PublicClass,
            ParameterName: $"{operationName}{ContentGeneratorConstants.Parameters}",
            SortOptionalParametersMustAppearAfterAllRequiredParameters(parameters));
    }

    private static void AppendParameters(
        ICollection<ContentGeneratorServerParameterParametersProperty> parameters,
        IEnumerable<OpenApiParameter> openApiParameters,
        string contractNamespaceWithoutApiGroupName)
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
                var defaultValueInitializer = openApiParameter.GetDefaultValueInitializer(contractNamespaceWithoutApiGroupName);

                parameters.Add(new ContentGeneratorServerParameterParametersProperty(
                    openApiParameter.Name,
                    openApiParameter.Name.EnsureValidFormattedPropertyName(),
                    openApiParameter.ExtractDocumentationTags(),
                    ConvertToParameterLocationType(openApiParameter.In),
                    dataType,
                    isSimpleType,
                    useListForDataType,
                    GetIsNullable(openApiParameter),
                    openApiParameter.Required,
                    GetAdditionalValidationAttributes(openApiParameter),
                    defaultValueInitializer));
            }
        }
    }

    private static bool GetIsNullable(
        OpenApiParameter openApiParameter)
    {
        var isNullable = openApiParameter.Schema.Nullable;
        if (isNullable)
        {
            if (openApiParameter.Required ||
                openApiParameter.Schema.IsSchemaEnumOrPropertyEnum())
            {
                isNullable = false;
            }
        }
        else
        {
            if (!openApiParameter.Required &&
                openApiParameter.In is
                    ParameterLocation.Query or
                    ParameterLocation.Header or
                    ParameterLocation.Cookie &&
                string.IsNullOrEmpty(openApiParameter.Schema.GetDefaultValueAsString()))
            {
                isNullable = true;
            }
        }

        return isNullable;
    }

    private static void AppendParametersFromBody(
        ICollection<ContentGeneratorServerParameterParametersProperty> parameters,
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
            requestBodyType = requestSchema.Reference.Id.PascalCase(ApiOperationExtractor.ModelNameSeparators, removeSeparators: true);
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
            requestBodyType = requestSchema.Items.Reference.Id.PascalCase(ApiOperationExtractor.ModelNameSeparators, removeSeparators: true);
        }

        parameters.Add(new ContentGeneratorServerParameterParametersProperty(
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

    /// <summary>
    /// CS1737 -  Sorts the optional parameters must appear after all required parameters.
    /// </summary>
    /// <param name="parameters">The parameter base parameters.</param>
    private static IList<ContentGeneratorServerParameterParametersProperty> SortOptionalParametersMustAppearAfterAllRequiredParameters(
        IReadOnlyCollection<ContentGeneratorServerParameterParametersProperty> parameters)
    {
        var parametersWithoutDefaultValues = parameters.Where(x => x.DefaultValueInitializer is null).ToList();
        var parametersWitDefaultValues = parameters.Where(x => x.DefaultValueInitializer is not null).ToList();

        var data = new List<ContentGeneratorServerParameterParametersProperty>();
        data.AddRange(parametersWithoutDefaultValues);
        data.AddRange(parametersWitDefaultValues);
        return data;
    }
}