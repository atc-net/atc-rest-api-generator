namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiOperationExtensions
{
    public static CodeDocumentationTags ExtractDocumentationTagsForEndpointInterface(
        this OpenApiOperation apiOperation)
        => apiOperation.ExtractDocumentationTags("Interface for Client Endpoint.");

    public static CodeDocumentationTags ExtractDocumentationTagsForEndpointResultInterface(
        this OpenApiOperation apiOperation)
        => apiOperation.ExtractDocumentationTags("Interface for Client Endpoint Result.");

    public static CodeDocumentationTags ExtractDocumentationTagsForEndpoint(
        this OpenApiOperation apiOperation)
        => apiOperation.ExtractDocumentationTags("Client Endpoint.");

    public static CodeDocumentationTags ExtractDocumentationTagsForEndpointResult(
        this OpenApiOperation apiOperation)
        => apiOperation.ExtractDocumentationTags("Client Endpoint result.");

    public static CodeDocumentationTags ExtractDocumentationTagsForHandlerInterface(
        this OpenApiOperation apiOperation)
        => apiOperation.ExtractDocumentationTags("Domain Interface for RequestHandler.");

    public static CodeDocumentationTags ExtractDocumentationTagsForHandler(
        this OpenApiOperation apiOperation)
        => apiOperation.ExtractDocumentationTags("Handler for operation request.");

    public static CodeDocumentationTags ExtractDocumentationTagsForParameters(
        this OpenApiOperation apiOperation)
        => apiOperation.ExtractDocumentationTags("Parameters for operation request.");

    public static CodeDocumentationTags ExtractDocumentationTagsForResult(
        this OpenApiOperation apiOperation)
        => apiOperation.ExtractDocumentationTags("Results for operation request.");

    public static CodeDocumentationTags ExtractDocumentationTags(
        this OpenApiOperation apiOperation,
        string? firstSummaryLine = null)
    {
        var summary = ContentGeneratorConstants.UndefinedDescription;

        if (!string.IsNullOrEmpty(apiOperation.Summary))
        {
            summary = apiOperation.Summary;
        }
        else if (!string.IsNullOrEmpty(apiOperation.Description))
        {
            summary = apiOperation.Description;
        }

        var sbSummary = new StringBuilder();

        if (!string.IsNullOrEmpty(firstSummaryLine))
        {
            sbSummary.AppendLine(firstSummaryLine);
        }

        sbSummary.AppendLine($"Description: {summary.EnsureEndsWithDot()}");
        sbSummary.AppendLine($"Operation: {apiOperation.GetOperationName()}.");

        return new CodeDocumentationTags(sbSummary.ToString());
    }

    public static ApiAuthorizeModel? ExtractApiOperationAuthorization(
        this OpenApiOperation apiOperation,
        OpenApiPathItem apiPath)
    {
        var authorizationRequiredForPath = apiPath.Extensions.ExtractAuthenticationRequired();
        var authorizationRolesForPath = apiPath.Extensions.ExtractAuthorizationRoles();
        var authenticationSchemesForPath = apiPath.Extensions.ExtractAuthenticationSchemes();
        var authorizationRequiredForOperation = apiOperation.Extensions.ExtractAuthenticationRequired();
        var authorizationRolesForOperation = apiOperation.Extensions.ExtractAuthorizationRoles();
        var authenticationSchemesForOperation = apiOperation.Extensions.ExtractAuthenticationSchemes();

        if (authorizationRequiredForPath.HasNoValueOrFalse() &&
            authorizationRolesForPath.Count == 0 &&
            authenticationSchemesForPath.Count == 0 &&
            authorizationRequiredForOperation.HasNoValue() &&
            authorizationRolesForOperation.Count == 0 &&
            authenticationSchemesForOperation.Count == 0)
        {
            return null;
        }

        IList<string>? authorizationRoles = null;
        IList<string>? authenticationSchemes = null;

        if (authorizationRolesForPath.Count > 0)
        {
            authorizationRoles = authorizationRolesForPath
                .Distinct(StringComparer.Ordinal)
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToList();
        }

        if (authorizationRolesForOperation.Count > 0)
        {
            authorizationRoles = authorizationRoles is null
                ? authorizationRolesForOperation
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(x => x, StringComparer.Ordinal)
                    .ToList()
                : authorizationRoles
                    .Union(authorizationRolesForOperation, StringComparer.Ordinal)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(x => x, StringComparer.Ordinal)
                    .ToList();
        }

        if (authenticationSchemesForPath.Count > 0)
        {
            authenticationSchemes = authenticationSchemesForPath
                .Distinct(StringComparer.Ordinal)
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToList();
        }

        if (authenticationSchemesForOperation.Count > 0)
        {
            authenticationSchemes = authenticationSchemes is null
                ? authenticationSchemesForOperation
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(x => x, StringComparer.Ordinal)
                    .ToList()
                : authenticationSchemes
                    .Union(authenticationSchemesForOperation, StringComparer.Ordinal)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(x => x, StringComparer.Ordinal)
                    .ToList();
        }

        var useAllowAnonymous = authorizationRequiredForPath.HasValueAndFalse() &&
                                authorizationRequiredForOperation.HasValueAndFalse();

        if (authorizationRoles?.Count > 0 ||
            authenticationSchemes?.Count > 0)
        {
            useAllowAnonymous = false;
        }

        if (authorizationRequiredForOperation.HasValueAndFalse())
        {
            useAllowAnonymous = true;
        }

        return new ApiAuthorizeModel(
            Roles: authorizationRoles,
            AuthenticationSchemes: authenticationSchemes,
            UseAllowAnonymous: useAllowAnonymous);
    }

    public static IEnumerable<ApiOperationResponseModel> ExtractApiOperationResponseModels(
        this OpenApiOperation apiOperation,
        string? @namespace = null)
    {
        var result = new List<ApiOperationResponseModel>();

        foreach (var apiResponse in apiOperation.Responses)
        {
            if (apiResponse.Value.Content.Count == 0)
            {
                if (!apiResponse.Key.TryParseToHttpStatusCode(out var httpStatusCode))
                {
                    continue;
                }

                result.Add(
                    new ApiOperationResponseModel(
                        StatusCode: httpStatusCode,
                        OperationName: apiOperation.GetOperationName(),
                        GroupName: null,
                        MediaType: null,
                        CollectionDataType: null,
                        DataType: null,
                        Description: apiResponse.Value.Description,
                        Namespace: null));
            }
            else
            {
                foreach (var apiMediaType in apiResponse.Value.Content.Where(x => !x.Key.Equals(MediaTypeNames.Application.Xml, StringComparison.OrdinalIgnoreCase)))
                {
                    if (!apiResponse.Key.TryParseToHttpStatusCode(out var httpStatusCode))
                    {
                        continue;
                    }

                    string? collectionDataType = null;
                    var useAsyncEnumerable = false;
                    var dataType = apiMediaType.Value.Schema.GetModelName();

                    if (string.IsNullOrEmpty(dataType) &&
                        apiMediaType.Value.Schema.IsSimpleDataType())
                    {
                        dataType = apiMediaType.Value.Schema.GetDataType();
                    }

                    if (string.IsNullOrEmpty(dataType))
                    {
                        dataType = null;

                        if (apiMediaType.Value.Schema.IsTypeCustomPagination())
                        {
                            var customPaginationItemsSchema = apiMediaType.Value.Schema.GetCustomPaginationItemsSchema();
                            if (customPaginationItemsSchema is not null)
                            {
                                dataType = customPaginationItemsSchema.GetModelName();
                                if (dataType.Length == 0 && apiMediaType.Value.Schema.IsSimpleDataType())
                                {
                                    dataType = apiMediaType.Value.Schema.GetDataType();
                                }

                                var customPaginationSchema = apiMediaType.Value.Schema.GetCustomPaginationSchema();
                                collectionDataType = customPaginationSchema is not null
                                    ? customPaginationSchema.GetDataType()
                                    : NameConstants.Pagination + ContentGeneratorConstants.Result;

                                useAsyncEnumerable = apiOperation.IsAsyncEnumerableEnabled();
                            }
                        }
                    }
                    else
                    {
                        collectionDataType = apiMediaType.Value.Schema.GetCollectionDataType();

                        useAsyncEnumerable = apiOperation.IsAsyncEnumerableEnabled();
                    }

                    result.Add(
                        new ApiOperationResponseModel(
                            StatusCode: httpStatusCode,
                            OperationName: apiOperation.GetOperationName(),
                            GroupName: null,
                            MediaType: apiMediaType.Key,
                            CollectionDataType: collectionDataType,
                            UseAsyncEnumerable: useAsyncEnumerable,
                            DataType: dataType,
                            Description: apiResponse.Value.Description,
                            Namespace: @namespace));
                }
            }
        }

        return result;
    }

    public static bool IsAsyncEnumerableEnabled(
        this OpenApiOperation operation)
        => operation.Extensions.TryGetValue("x-return-async-enumerable", out var extension) &&
           extension is OpenApiBoolean { Value: true };
}