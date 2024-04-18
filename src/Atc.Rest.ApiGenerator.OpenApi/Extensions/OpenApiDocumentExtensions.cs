// ReSharper disable InvertIf
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiDocumentExtensions
{
    public static bool IsSpecificationUsingAuthorization(
        this OpenApiDocument openApiDocument)
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiPathAuthentication = openApiPath.Value.Extensions.ExtractAuthenticationRequired();
            if (apiPathAuthentication is not null && apiPathAuthentication.Value)
            {
                return true;
            }

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                var apiOperationAuthentication = openApiOperation.Value.Extensions.ExtractAuthenticationRequired();
                if (apiOperationAuthentication is not null && apiOperationAuthentication.Value)
                {
                    return true;
                }
            }
        }

        return false;
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

    public static bool IsUsingRequiredForSystemNet(
        this OpenApiDocument openApiDocument,
        bool useProblemDetailsAsDefaultBody)
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
                    if (httpStatusCode == HttpStatusCode.OK)
                    {
                        continue;
                    }

                    var useProblemDetails = openApiOperation.Responses.IsSchemaTypeProblemDetailsForStatusCode(httpStatusCode);
                    if (!useProblemDetails &&
                        !useProblemDetailsAsDefaultBody)
                    {
                        continue;
                    }

                    return true;
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
}