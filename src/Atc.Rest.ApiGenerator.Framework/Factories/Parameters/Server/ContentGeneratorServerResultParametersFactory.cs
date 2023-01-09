// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Server;

public static class ContentGeneratorServerResultParametersFactory
{
    public static ContentGeneratorServerResultParameters Create(
        string @namespace,
        OpenApiOperation openApiOperation,
        bool useProblemDetailsAsDefaultBody)
    {
        ArgumentNullException.ThrowIfNull(openApiOperation);

        var operationName = openApiOperation.GetOperationName();
        var httpStatusCodes = openApiOperation.Responses.GetHttpStatusCodes();

        var schemaTypeForImplicitOperator = SchemaType.None;
        string? modelNameForImplicitOperator = null;
        string? simpleDataTypeNameForImplicitOperator = null;

        // Methods
        var methodParameters = new List<ContentGeneratorServerResultMethodParameters>();
        foreach (var httpStatusCode in httpStatusCodes)
        {
            // TODO: Refactor to method
            var useProblemDetails = openApiOperation.Responses.IsSchemaTypeProblemDetailsForStatusCode(httpStatusCode);
            if (!useProblemDetails &&
                useProblemDetailsAsDefaultBody)
            {
                useProblemDetails = true;
            }

            var schemaType = GetSchemaType(openApiOperation, httpStatusCode);
            if (httpStatusCode is HttpStatusCode.OK or HttpStatusCode.Created)
            {
                schemaTypeForImplicitOperator = schemaType;
            }

            var simpleDataTypeName = GetSimpleDataTypeName(schemaType, openApiOperation, httpStatusCode);
            if (httpStatusCode is HttpStatusCode.OK or HttpStatusCode.Created && !string.IsNullOrEmpty(simpleDataTypeName))
            {
                if ("Object".Equals(simpleDataTypeName, StringComparison.Ordinal))
                {
                    simpleDataTypeName = "object";
                }

                simpleDataTypeNameForImplicitOperator = simpleDataTypeName;
            }

            var modelNameForStatusCode = openApiOperation.Responses.GetModelNameForStatusCode(httpStatusCode);
            if (httpStatusCode is HttpStatusCode.OK or HttpStatusCode.Created && !string.IsNullOrEmpty(modelNameForStatusCode))
            {
                modelNameForImplicitOperator = modelNameForStatusCode;
            }

            var documentationTags = new CodeDocumentationTags($"{(int)httpStatusCode} - {httpStatusCode.ToNormalizedString()} response.");

            methodParameters.Add(new ContentGeneratorServerResultMethodParameters(
                httpStatusCode,
                schemaType,
                UsesProblemDetails: useProblemDetails,
                ModelName: modelNameForStatusCode,
                documentationTags,
                UsesBinaryResponse: httpStatusCode == HttpStatusCode.OK
                    ? openApiOperation.Responses.IsSchemaUsingBinaryFormatForOkResponse()
                    : null,
                SimpleDataTypeName: simpleDataTypeName));
        }

        ContentGeneratorServerResultImplicitOperatorParameters? implicitOperatorParameters = null;

        if (ShouldAppendImplicitOperatorContent(httpStatusCodes, modelNameForImplicitOperator, openApiOperation.Responses.IsSchemaUsingBinaryFormatForOkResponse()))
        {
            // Implicit Operator
            implicitOperatorParameters = new ContentGeneratorServerResultImplicitOperatorParameters(
                SchemaType: schemaTypeForImplicitOperator,
                modelNameForImplicitOperator,
                simpleDataTypeNameForImplicitOperator);
        }

        return new ContentGeneratorServerResultParameters(
            @namespace,
            operationName,
            openApiOperation.ExtractDocumentationTagsForResult(),
            $"{operationName}{ContentGeneratorConstants.Result}",
            methodParameters,
            implicitOperatorParameters);
    }

    private static bool ShouldAppendImplicitOperatorContent(
        ICollection<HttpStatusCode> httpStatusCodes,
        string? modelName,
        bool isSchemaUsingBinaryFormatForOkResponse)
    {
        if (!httpStatusCodes.Contains(HttpStatusCode.OK) &&
            !httpStatusCodes.Contains(HttpStatusCode.Created))
        {
            return false;
        }

        if (httpStatusCodes.Contains(HttpStatusCode.OK) &&
            httpStatusCodes.Contains(HttpStatusCode.Created))
        {
            return false;
        }

        var httpStatusCode = HttpStatusCode.Continue; // Dummy
        if (httpStatusCodes.Contains(HttpStatusCode.OK))
        {
            httpStatusCode = HttpStatusCode.OK;
        }
        else if (httpStatusCodes.Contains(HttpStatusCode.Created))
        {
            httpStatusCode = HttpStatusCode.Created;
        }

        if (string.IsNullOrEmpty(modelName) &&
            httpStatusCode == HttpStatusCode.Created)
        {
            return false;
        }

        if (isSchemaUsingBinaryFormatForOkResponse)
        {
            return false;
        }

        return true;
    }

    private static SchemaType GetSchemaType(
        OpenApiOperation openApiOperation,
        HttpStatusCode httpStatusCode)
    {
        var schema = openApiOperation.Responses.GetSchemaForStatusCode(httpStatusCode);
        if (schema is null)
        {
            return SchemaType.None;
        }

        var modelName = openApiOperation.Responses.GetModelNameForStatusCode(httpStatusCode);
        if (!string.IsNullOrEmpty(modelName))
        {
            if (schema.IsTypeArray())
            {
                return SchemaType.ComplexTypeList;
            }

            if (schema.IsTypePagination())
            {
                return SchemaType.ComplexTypePagedList;
            }

            return SchemaType.ComplexType;
        }

        if (schema.IsTypeArray() && schema.HasItemsWithSimpleDataType())
        {
            return SchemaType.SimpleTypeList;
        }

        if (schema.IsTypePagination() && schema.HasPaginationItemsWithSimpleDataType())
        {
            return SchemaType.SimpleTypePagedList;
        }

        return SchemaType.SimpleType;
    }

    private static string? GetSimpleDataTypeName(
        SchemaType schemaType,
        OpenApiOperation openApiOperation,
        HttpStatusCode httpStatusCode)
    {
        var schema = openApiOperation.Responses.GetSchemaForStatusCode(httpStatusCode);
        if (schema is null)
        {
            return null;
        }

        return schemaType switch
        {
            SchemaType.SimpleType => schema.GetDataType(),
            SchemaType.SimpleTypeList => schema.GetSimpleDataTypeFromArray(),
            SchemaType.SimpleTypePagedList => schema.GetSimpleDataTypeFromPagination(),
            _ => null,
        };
    }
}