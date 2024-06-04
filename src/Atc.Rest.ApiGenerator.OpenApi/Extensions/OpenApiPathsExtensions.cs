namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiPathsExtensions
{
    public static string GetApiGroupName(
        this KeyValuePair<string, OpenApiPathItem> apiPath)
    {
        var sa = apiPath.Key.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return sa.Length == 0
            ? "Root"
            : sa[0].PascalCase(removeSeparators: true);
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

        var useAllowAnonymous = apiPath.Operations.Keys.Count != data.OfType<ApiAuthorizeModel>().Count();

        return new ApiAuthorizeModel(
            Roles: authorizationRoles,
            AuthenticationSchemes: authenticationSchemes,
            UseAllowAnonymous: useAllowAnonymous);
    }
}