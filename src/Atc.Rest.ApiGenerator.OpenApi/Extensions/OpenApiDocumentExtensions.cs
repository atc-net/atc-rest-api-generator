// ReSharper disable InvertIf
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiDocumentExtensions
{
    public static IEnumerable<ApiOperationResponseModel> ExtractApiOperationResponseModels(
        this OpenApiDocument openApiDocument)
    {
        var result = new List<ApiOperationResponseModel>();

        foreach (var apiPath in openApiDocument.Paths)
        {
            var apiGroupName = apiPath.GetApiGroupName();

            foreach (var apiOperation in apiPath.Value.Operations)
            {
                foreach (var responseModel in apiOperation.Value.ExtractApiOperationResponseModels())
                {
                    result.Add(responseModel with { GroupName = apiGroupName });
                }
            }
        }

        return result;
    }

    public static bool HasAllPathsAuthenticationRequiredSet(
        this OpenApiDocument openApiDocument,
        string area)
    {
        foreach (var (_, apiPathData) in openApiDocument.GetPathsByBasePathSegmentName(area))
        {
            var apiPathAuthenticationRequired = apiPathData.Extensions.ExtractAuthenticationRequired();
            if (apiPathAuthenticationRequired is null ||
                !apiPathAuthenticationRequired.Value)
            {
                return false;
            }
        }

        return true;
    }

    public static SwaggerDocOptionsParameters ToSwaggerDocOptionsParameters(
        this OpenApiDocument openApiDocument)
        => new(
            openApiDocument.Info?.Version,
            openApiDocument.Info?.Title,
            openApiDocument.Info?.Description,
            openApiDocument.Info?.Contact?.Name,
            openApiDocument.Info?.Contact?.Email,
            openApiDocument.Info?.Contact?.Url?.ToString(),
            openApiDocument.Info?.TermsOfService?.ToString(),
            openApiDocument.Info?.License?.Name,
            openApiDocument.Info?.License?.Url?.ToString());

    public static IList<string> GetApiGroupNames(
        this OpenApiDocument openApiDocument)
    {
        var names = new List<string>();
        if (openApiDocument.Paths?.Keys is null)
        {
            return names.ToList();
        }

        foreach (var path in openApiDocument.Paths)
        {
            var apiGroupName = path.GetApiGroupName();
            if (!names.Contains(apiGroupName, StringComparer.Ordinal))
            {
                names.Add(apiGroupName);
            }
        }

        return names
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();
    }

    [SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "OK.")]
    public static string GetServerUrlBasePath(
        this OpenApiDocument openApiDocument)
    {
        var serverUrl = openApiDocument.Servers?.FirstOrDefault()?.Url;
        if (string.IsNullOrWhiteSpace(serverUrl))
        {
            return "/api/v1";
        }

        serverUrl = serverUrl.Replace("//", "/", StringComparison.Ordinal);
        serverUrl = serverUrl.Replace("http:/", "http://", StringComparison.OrdinalIgnoreCase);
        serverUrl = serverUrl.Replace("https:/", "https://", StringComparison.OrdinalIgnoreCase);
        if (!serverUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
            !serverUrl.StartsWith('/'))
        {
            serverUrl = $"/{serverUrl}";
        }

        if (serverUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            serverUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            var temp = serverUrl
                .Replace("http://", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("https://", string.Empty, StringComparison.OrdinalIgnoreCase);

            return temp.Substring(temp.IndexOf('/', StringComparison.Ordinal));
        }

        return serverUrl;
    }

    public static bool IsUsingRequiredForSystem(
        this OpenApiDocument openApiDocument)
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated)
                {
                    continue;
                }

                foreach (var response in openApiOperation.Value.Responses.Values)
                {
                    foreach (var mediaType in response.Content.Values)
                    {
                        foreach (var schemaProperty in mediaType.Schema.Properties)
                        {
                            if (schemaProperty.Value.IsFormatTypeUuid())
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public static bool IsUsingRequiredForSystemCollectionGeneric(
        this OpenApiDocument openApiDocument)
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            foreach (var apiOperation in openApiPath.Value.Operations)
            {
                foreach (var response in apiOperation.Value.Responses.Values)
                {
                    foreach (var mediaType in response.Content.Values)
                    {
                        if (mediaType.Schema.IsTypeArray())
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public static bool IsUsingRequiredForSystemLinq(
        this OpenApiDocument openApiDocument)
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            foreach (var apiOperation in openApiPath.Value.Operations)
            {
                foreach (var parameter in apiOperation.Value.Parameters)
                {
                    if (parameter.Schema.IsTypeArray())
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool IsUsingRequiredForAtcRestResults(
        this OpenApiDocument openApiDocument)
    {
        foreach (var path in openApiDocument.Paths)
        {
            foreach (var openApiOperation in path.Value.Operations.Values)
            {
                foreach (var response in openApiOperation.Responses.OrderBy(x => x.Key, StringComparer.Ordinal))
                {
                    if (!Enum.TryParse(typeof(HttpStatusCode), response.Key, out var parsedType))
                    {
                        continue;
                    }

                    var httpStatusCode = (HttpStatusCode)parsedType;
                    if (httpStatusCode != HttpStatusCode.OK)
                    {
                        continue;
                    }

                    var usePagination = openApiOperation.Responses.IsSchemaTypePaginationForStatusCode(httpStatusCode);
                    if (!usePagination)
                    {
                        continue;
                    }

                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsUsingRequiredForAtcRestMinimalApiFiltersEndpoints(
        this OpenApiDocument openApiDocument)
    {
        // TODO: Check for any use of operations parameters
        return true;
    }

    public static bool IsUsingRequiredForMicrosoftAspNetCoreAuthorization(
        this OpenApiDocument openApiDocument)
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var isAuthenticationRequired = openApiPath.Value.Extensions.ExtractAuthenticationRequired();
            if (isAuthenticationRequired is not null && isAuthenticationRequired.Value)
            {
                return true;
            }

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                var isOperationAuthenticationRequired = openApiOperation.Value.Extensions.ExtractAuthenticationRequired();
                if (isOperationAuthenticationRequired is not null && isOperationAuthenticationRequired.Value)
                {
                    return true;
                }

                var operationAuthenticationRoles = openApiPath.Value.Extensions.ExtractAuthorizationRoles();
                var operationAuthenticationSchemes = openApiPath.Value.Extensions.ExtractAuthenticationSchemes();
                if (operationAuthenticationRoles is not null && operationAuthenticationRoles.Count > 0 &&
                    operationAuthenticationSchemes is not null && operationAuthenticationSchemes.Count > 0)
                {
                    return true;
                }
            }

            var authenticationRoles = openApiPath.Value.Extensions.ExtractAuthorizationRoles();
            var authenticationSchemes = openApiPath.Value.Extensions.ExtractAuthenticationSchemes();
            if (authenticationRoles is not null && authenticationRoles.Count > 0 &&
                authenticationSchemes is not null && authenticationSchemes.Count > 0)
            {
                return true;
            }
        }

        return false;
    }
}