// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable MergeIntoPattern
namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Server;

public static class ContentGeneratorServerParameterParametersFactory
{
    public static ContentGeneratorServerParameterParameters Create(
        string @namespace,
        OpenApiOperation openApiOperation,
        IList<OpenApiParameter> globalPathParameters)
    {
        ArgumentNullException.ThrowIfNull(openApiOperation);
        ArgumentNullException.ThrowIfNull(globalPathParameters);

        var operationName = openApiOperation.GetOperationName();

        var parameters = new List<ContentGeneratorServerParameterParametersProperty>();

        AppendParameters(parameters, globalPathParameters);
        AppendParameters(parameters, openApiOperation.Parameters);
        AppendParametersFromBody(parameters, openApiOperation.RequestBody);

        return new ContentGeneratorServerParameterParameters(
            @namespace,
            operationName,
            openApiOperation.ExtractDocumentationTagsForParameters(),
            ParameterName: $"{operationName}{ContentGeneratorConstants.Parameters}",
            parameters);
    }

    private static void AppendParameters(
        ICollection<ContentGeneratorServerParameterParametersProperty> parameters,
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
                parameters.Add(new ContentGeneratorServerParameterParametersProperty(
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
}