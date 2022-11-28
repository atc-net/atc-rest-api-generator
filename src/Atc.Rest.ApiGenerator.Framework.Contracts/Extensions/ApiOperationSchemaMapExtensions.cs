namespace Atc.Rest.ApiGenerator.Framework.Contracts.Extensions;

public static class ApiOperationSchemaMapExtensions
{
    public static bool IsShared(
        this IList<ApiOperation> apiOperationSchemaMaps,
        string schemaKey)
    {
        var apiOperationSchemaMap = apiOperationSchemaMaps.FirstOrDefault(x => x.Model.Name.Equals(schemaKey, StringComparison.Ordinal));
        return apiOperationSchemaMap is not null &&
               apiOperationSchemaMap.Model.IsShared;
    }
}