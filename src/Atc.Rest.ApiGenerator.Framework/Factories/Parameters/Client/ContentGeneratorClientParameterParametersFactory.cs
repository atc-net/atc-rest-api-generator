// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable MergeIntoPattern
namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Client;

public static class ContentGeneratorClientParameterParametersFactory
{
    public static ContentGeneratorClientParameterParameters Create(
        string @namespace,
        OpenApiOperation openApiOperation,
        IList<OpenApiParameter> globalPathParameters)
    {
        ArgumentNullException.ThrowIfNull(openApiOperation);
        ArgumentNullException.ThrowIfNull(globalPathParameters);

        var operationName = openApiOperation.GetOperationName();

        var parameters = new List<ContentGeneratorClientParameterParametersProperty>();

        AppendParameters(parameters, globalPathParameters);
        AppendParameters(parameters, openApiOperation.Parameters);
        AppendParametersFromBody(parameters, openApiOperation.RequestBody);

        return new ContentGeneratorClientParameterParameters(
            @namespace,
            operationName,
            openApiOperation.ExtractDocumentationTagsForParameters(),
            ParameterName: $"{operationName}{ContentGeneratorConstants.Parameters}",
            parameters);
    }

    private static void AppendParameters(
        ICollection<ContentGeneratorClientParameterParametersProperty> parameters,
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
                parameters.Add(new ContentGeneratorClientParameterParametersProperty(
                    openApiParameter.Name,
                    openApiParameter.Name.EnsureFirstCharacterToUpper(),
                    openApiParameter.ExtractDocumentationTags(),
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
        ICollection<ContentGeneratorClientParameterParametersProperty> parameters,
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

        parameters.Add(new ContentGeneratorClientParameterParametersProperty(
            string.Empty,
            ContentGeneratorConstants.Request,
            requestSchema.ExtractDocumentationTags(),
            requestBodyType,
            IsSimpleType: false,
            UseListForDataType: requestSchema.IsTypeArray(),
            IsNullable: false,
            IsRequired: true,
            AdditionalValidationAttributes: new List<ValidationAttribute>(),
            DefaultValueInitializer: null));
    }

    private static IList<ValidationAttribute> GetAdditionalValidationAttributes(
        OpenApiParameter openApiParameter)
    {
        var validationAttributeExtractor = new ValidationAttributeExtractor();
        return validationAttributeExtractor.Extract(openApiParameter.Schema);
    }
}