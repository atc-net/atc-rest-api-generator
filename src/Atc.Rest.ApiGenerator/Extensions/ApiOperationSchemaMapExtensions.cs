namespace Atc.Rest.ApiGenerator.Extensions;

internal static class ApiOperationSchemaMapExtensions
{
    public static bool IsShared(
        this IList<ApiOperationSchemaMap> apiOperationSchemaMaps,
        string schemaKey)
    {
        var apiOperationSchemaMap = apiOperationSchemaMaps.FirstOrDefault(x => x.SchemaKey.Equals(schemaKey, StringComparison.Ordinal));
        return apiOperationSchemaMap is not null &&
               apiOperationSchemaMap.IsShared;
    }
}