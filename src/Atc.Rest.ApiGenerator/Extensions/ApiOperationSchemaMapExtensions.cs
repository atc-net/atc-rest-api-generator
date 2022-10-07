namespace Atc.Rest.ApiGenerator.Extensions;

internal static class ApiOperationSchemaMapExtensions
{
    public static bool IsShared(
        this IList<ApiOperationSchemaMap> apiOperationSchemaMaps,
        string schemaKey)
    {
        var maps = apiOperationSchemaMaps.Where(x => x.SchemaKey == schemaKey).ToList();

        var segmentNames = new List<string>();
        foreach (var s in maps
                     .Select(map => map.SegmentName)
                     .Where(s => !segmentNames.Contains(s, StringComparer.Ordinal)))
        {
            segmentNames.Add(s);
        }

        return segmentNames.Count > 1;
    }
}