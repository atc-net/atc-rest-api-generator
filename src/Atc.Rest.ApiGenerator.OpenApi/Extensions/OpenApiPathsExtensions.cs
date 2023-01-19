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
}