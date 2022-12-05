namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters;

public static class ContentGeneratorServerResultParametersFactory
{
    public static ContentGeneratorServerResultParameters Create(
        string @namespace,
        OpenApiOperation openApiOperation,
        bool useProblemDetailsAsDefaultBody)
    {
        var operationName = openApiOperation.GetOperationName();

        // Methods
        var methodParameters = new List<ContentGeneratorServerResultMethodParameters>();
        foreach (var httpStatusCode in openApiOperation.Responses.GetHttpStatusCodes())
        {
            ////bool? usesBinaryResponse;

            var isList = openApiOperation.Responses.IsSchemaTypeArrayForStatusCode(httpStatusCode);
            var isPagination = openApiOperation.Responses.IsSchemaTypePaginationForStatusCode(httpStatusCode);

            // TODO: Refactor to method
            var useProblemDetails = openApiOperation.Responses.IsSchemaTypeProblemDetailsForStatusCode(httpStatusCode);
            if (!useProblemDetails &&
                useProblemDetailsAsDefaultBody)
            {
                useProblemDetails = true;
            }

            methodParameters.Add(new ContentGeneratorServerResultMethodParameters(
                httpStatusCode,
                GetSchemaType(openApiOperation, httpStatusCode),
                UsesProblemDetails: useProblemDetails,
                ModelName: openApiOperation.Responses.GetModelNameForStatusCode(httpStatusCode),
                UsesBinaryResponse: null)); // TODO: Extract)
        }

        // Implicit Operators
        var implicitOperatorParameters = new List<ContentGeneratorServerResultImplicitOperatorParameters>();

        return new ContentGeneratorServerResultParameters(
            @namespace,
            operationName,
            openApiOperation.GetOperationSummaryDescription(),
            $"{operationName}{ContentGeneratorConstants.Result}",
            methodParameters,
            implicitOperatorParameters);
    }

    private static SchemaType GetSchemaType(
        OpenApiOperation openApiOperation,
        HttpStatusCode httpStatusCode)
    {
        var isList = openApiOperation.Responses.IsSchemaTypeArrayForStatusCode(httpStatusCode);
        if (isList)
        {
            return SchemaType.List;
        }

        var isPagination = openApiOperation.Responses.IsSchemaTypePaginationForStatusCode(httpStatusCode);
        if (isPagination)
        {
            return SchemaType.PagedList;
        }

        // TODO: Implement

        return SchemaType.Unknown;
    }
}