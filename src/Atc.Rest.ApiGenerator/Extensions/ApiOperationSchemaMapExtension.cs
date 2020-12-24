using System.Collections.Generic;
using System.Linq;
using Atc.Rest.ApiGenerator.Models;

namespace Atc.Rest.ApiGenerator.Extensions
{
    internal static class ApiOperationSchemaMapExtension
    {
        public static bool IsShared(this List<ApiOperationSchemaMap> apiOperationSchemaMaps, string schemaKey)
        {
            var maps = apiOperationSchemaMaps.Where(x => x.SchemaKey == schemaKey).ToList();

            var segmentNames = new List<string>();
            foreach (var s in maps
                .Select(map => map.SegmentName)
                .Where(s => !segmentNames.Contains(s)))
            {
                segmentNames.Add(s);
            }

            return segmentNames.Count > 1;
        }
    }
}