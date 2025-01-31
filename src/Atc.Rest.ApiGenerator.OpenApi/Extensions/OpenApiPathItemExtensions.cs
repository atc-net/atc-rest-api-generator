namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiPathItemExtensions
{
    [SuppressMessage("Naming", "CA1867:Use 'string.Method(char)' instead of 'string.Method(string)' for string with single char", Justification = "OK.")]
    public static string GetApiGroupName(
        this KeyValuePair<string, OpenApiPathItem> apiPath)
    {
        var sa = apiPath.Key.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return sa.Length switch
        {
            0 => "Root",
            > 2 when sa[0].Equals("api", StringComparison.OrdinalIgnoreCase) &&
                     sa[1].StartsWith("v", StringComparison.OrdinalIgnoreCase) => sa[2].PascalCase(removeSeparators: true),
            _ => sa[0].PascalCase(removeSeparators: true),
        };
    }

    public static bool IsPathStartingApiGroupName(
        this KeyValuePair<string, OpenApiPathItem> urlPath, string segmentName)
    {
        if (segmentName is null)
        {
            throw new ArgumentNullException(nameof(segmentName));
        }

        var sa = urlPath.Key.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (string.IsNullOrEmpty(segmentName) && sa.Length == 0)
        {
            return true;
        }

        if (sa.Length == 0)
        {
            return false;
        }

        var apiGroupName = urlPath.GetApiGroupName();

        return segmentName.Equals(apiGroupName, StringComparison.OrdinalIgnoreCase) ||
               segmentName.Equals(apiGroupName.EnsureSingular(), StringComparison.OrdinalIgnoreCase);
    }

    public static ApiAuthorizeModel? ExtractApiPathAuthorization(
        this OpenApiPathItem apiPath)
    {
        var data = apiPath.Operations
            .Select(apiOperation => apiOperation.Value.ExtractApiOperationAuthorization(apiPath))
            .ToList();

        if (data.Count == 0)
        {
            return null;
        }

        IList<string>? authorizationRoles = null;
        IList<string>? authenticationSchemes = null;

        foreach (var authorizeModel in data.OfType<ApiAuthorizeModel>())
        {
            if (authorizeModel.Roles is not null)
            {
                authorizationRoles = authorizationRoles?
                    .Union(authorizeModel.Roles, StringComparer.Ordinal)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(x => x, StringComparer.Ordinal)
                    .ToList() ?? authorizeModel.Roles;
            }

            if (authorizeModel.AuthenticationSchemes is not null)
            {
                authenticationSchemes = authenticationSchemes?
                    .Union(authorizeModel.AuthenticationSchemes, StringComparer.Ordinal)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(x => x, StringComparer.Ordinal)
                    .ToList() ?? authorizeModel.AuthenticationSchemes;
            }
        }

        var authenticationRequiredForPath = apiPath.Extensions.ExtractAuthenticationRequired();

        if ((authorizationRoles is null || authorizationRoles.Count == 0) &&
            (authenticationSchemes is null || authenticationSchemes.Count == 0) &&
            authenticationRequiredForPath is null)
        {
            return null;
        }

        return new ApiAuthorizeModel(
            Roles: authorizationRoles,
            AuthenticationSchemes: authenticationSchemes,
            UseAllowAnonymous: false);
    }
}