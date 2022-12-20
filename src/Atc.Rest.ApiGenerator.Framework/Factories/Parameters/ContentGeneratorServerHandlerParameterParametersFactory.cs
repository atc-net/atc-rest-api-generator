// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters;

public static class ContentGeneratorServerHandlerParameterParametersFactory
{
    public static ContentGeneratorServerHandlerParameterParameters Create(
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

        return new ContentGeneratorServerHandlerParameterParameters(
            @namespace,
            operationName,
            openApiOperation.GetOperationSummaryDescription(),
            ParameterName: $"{operationName}{ContentGeneratorConstants.Parameters}",
            parameters);
    }

    private static void AppendParameters(
        ICollection<ContentGeneratorServerParameterParametersProperty> parameters,
        IEnumerable<OpenApiParameter> openApiParameters)
    {
        foreach (var openApiParameter in openApiParameters)
        {
            parameters.Add(new ContentGeneratorServerParameterParametersProperty(
                openApiParameter.Name,
                openApiParameter.Name.PascalCase(removeSeparators: true),
                openApiParameter.GetOperationSummaryDescription(),
                GetParameterLocationType(openApiParameter.In),
                openApiParameter.Schema.GetDataType(),
                openApiParameter.Schema.Nullable,
                openApiParameter.Required));
        }
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
            "Request",
            ContentGeneratorConstants.UndefinedDescription,
            ParameterLocationType.Body,
            requestBodyType,
            IsNullable: false,
            IsRequired: true));
    }

    private static ParameterLocationType GetParameterLocationType(
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
}