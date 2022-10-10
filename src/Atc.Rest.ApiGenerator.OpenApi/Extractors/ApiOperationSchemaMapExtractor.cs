namespace Atc.Rest.ApiGenerator.OpenApi.Extractors;

public sealed class ApiOperationSchemaMapExtractor : IApiOperationSchemaMapExtractor
{
    public IList<ApiOperationSchemaMap> Extract(
        OpenApiDocument apiDocument)
    {
        ArgumentNullException.ThrowIfNull(apiDocument);

        var componentsSchemas = apiDocument.Components.Schemas;

        var result = new List<ApiOperationSchemaMap>();
        foreach (var apiPath in apiDocument.Paths)
        {
            foreach (var apiOperation in apiPath.Value.Operations)
            {
                ExtractMappingsFromParameters(componentsSchemas, (apiOperation.Key.ToHttpOperationType(), apiOperation.Value), apiPath, result);
                ExtractMappingsFromRequestBody(componentsSchemas, (apiOperation.Key.ToHttpOperationType(), apiOperation.Value), apiPath, result);
                ExtractMappingsFromResponses(componentsSchemas, (apiOperation.Key.ToHttpOperationType(), apiOperation.Value), apiPath, result);
            }
        }

        SetIsShared(result);

        return result;
    }

    private static void ExtractMappingsFromParameters(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        (HttpOperationType HttpOperationType, OpenApiOperation OpenApiOperation) apiOperation,
        KeyValuePair<string, OpenApiPathItem> apiPath,
        List<ApiOperationSchemaMap> result)
    {
        foreach (var apiParameter in apiOperation.OpenApiOperation.Parameters)
        {
            if (apiParameter.Content is null)
            {
                continue;
            }

            foreach (var apiMediaType in apiParameter.Content)
            {
                CollectSchema(
                    componentsSchemas,
                    apiSchema: apiMediaType.Value.Schema,
                    locatedArea: ApiSchemaMapLocatedAreaType.Parameter,
                    apiPath.Key,
                    httpOperation: apiOperation.HttpOperationType,
                    parentApiSchema: null,
                    result);
            }
        }
    }

    private static void ExtractMappingsFromRequestBody(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        (HttpOperationType HttpOperationType, OpenApiOperation OpenApiOperation) apiOperation,
        KeyValuePair<string, OpenApiPathItem> apiPath,
        List<ApiOperationSchemaMap> result)
    {
        if (apiOperation.OpenApiOperation.RequestBody?.Content is null)
        {
            return;
        }

        foreach (var apiMediaType in apiOperation.OpenApiOperation.RequestBody.Content)
        {
            CollectSchema(
                componentsSchemas,
                apiSchema: apiMediaType.Value.Schema,
                locatedArea: ApiSchemaMapLocatedAreaType.RequestBody,
                apiPath.Key,
                apiOperation.HttpOperationType,
                parentApiSchema: null,
                result);
        }
    }

    private static void ExtractMappingsFromResponses(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        (HttpOperationType HttpOperationType, OpenApiOperation OpenApiOperation) apiOperation,
        KeyValuePair<string, OpenApiPathItem> apiPath,
        List<ApiOperationSchemaMap> result)
    {
        foreach (var apiResponse in apiOperation.OpenApiOperation.Responses)
        {
            if (apiResponse.Value?.Content is null)
            {
                continue;
            }

            foreach (var apiMediaType in apiResponse.Value.Content)
            {
                CollectSchema(
                    componentsSchemas,
                    apiSchema: apiMediaType.Value.Schema,
                    locatedArea: ApiSchemaMapLocatedAreaType.Response,
                    apiPath.Key,
                    apiOperation.HttpOperationType,
                    parentApiSchema: null,
                    result);
            }
        }
    }

    private static void CollectSchema(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        OpenApiSchema? apiSchema,
        ApiSchemaMapLocatedAreaType locatedArea,
        string apiPath,
        HttpOperationType httpOperation,
        string? parentApiSchema,
        List<ApiOperationSchemaMap> result)
    {
        if (apiSchema is null)
        {
            return;
        }

        (var schemaKey, apiSchema) = ConsolidateSchemaObjectTypes(apiSchema);

        if (schemaKey.Length == 0 ||
            schemaKey == nameof(ProblemDetails) ||
            schemaKey.Equals(NameConstants.Pagination, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var apiOperationSchemaMap = new ApiOperationSchemaMap(schemaKey, locatedArea, apiPath, httpOperation, parentApiSchema);
        if (result.Any(x => x.Equals(apiOperationSchemaMap)))
        {
            return;
        }

        result.Add(apiOperationSchemaMap);

        if (apiSchema.Properties.Any())
        {
            Collect(
                componentsSchemas,
                apiSchema.Properties.ToList(),
                locatedArea,
                apiPath,
                httpOperation,
                schemaKey,
                result);
        }
        else if (apiSchema.AllOf.Count == 2 &&
                 (NameConstants.Pagination.Equals(apiSchema.AllOf[0].Reference?.Id, StringComparison.OrdinalIgnoreCase) ||
                  NameConstants.Pagination.Equals(apiSchema.AllOf[1].Reference?.Id, StringComparison.OrdinalIgnoreCase)))
        {
            string? subSchemaKey = null;
            if (!NameConstants.Pagination.Equals(apiSchema.AllOf[0].Reference?.Id, StringComparison.OrdinalIgnoreCase))
            {
                subSchemaKey = apiSchema.AllOf[0].GetModelName();
            }

            if (!NameConstants.Pagination.Equals(apiSchema.AllOf[1].Reference?.Id, StringComparison.OrdinalIgnoreCase))
            {
                subSchemaKey = apiSchema.AllOf[1].GetModelName();
            }

            if (subSchemaKey is null)
            {
                return;
            }

            var subApiSchema = componentsSchemas.Single(x => x.Key.Equals(schemaKey, StringComparison.OrdinalIgnoreCase)).Value;

            Collect(
                componentsSchemas,
                subApiSchema.Properties.ToList(),
                locatedArea,
                apiPath,
                httpOperation,
                subSchemaKey,
                result);
        }
        else if (apiSchema.IsTypeArray() &&
                 apiSchema.Items?.Reference?.Id is not null)
        {
            var subSchemaKey = apiSchema.Items.GetModelName();
            var subApiOperationSchemaMap = new ApiOperationSchemaMap(subSchemaKey, locatedArea, apiPath, httpOperation, parentApiSchema);
            if (result.Any(x => x.Equals(subApiOperationSchemaMap)))
            {
                return;
            }

            result.Add(subApiOperationSchemaMap);

            var subApiSchema = componentsSchemas.Single(x => x.Key.Equals(subSchemaKey, StringComparison.OrdinalIgnoreCase)).Value;
            if (subApiSchema.Properties.Any())
            {
                Collect(
                    componentsSchemas,
                    subApiSchema.Properties.ToList(),
                    locatedArea,
                    apiPath,
                    httpOperation,
                    subSchemaKey,
                    result);
            }
        }
    }

    private static void Collect(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        IEnumerable<KeyValuePair<string, OpenApiSchema>> propertySchemas,
        ApiSchemaMapLocatedAreaType locatedArea,
        string apiPath,
        HttpOperationType httpOperation,
        string parentApiSchema,
        List<ApiOperationSchemaMap> result)
    {
        foreach (var propertySchema in propertySchemas)
        {
            CollectSchema(
                componentsSchemas,
                propertySchema.Value,
                locatedArea,
                apiPath,
                httpOperation,
                parentApiSchema,
                result);
        }
    }

    private static (string, OpenApiSchema) ConsolidateSchemaObjectTypes(
        OpenApiSchema apiSchema)
    {
        var schemaKey = string.Empty;
        if (apiSchema.Reference?.Id is not null)
        {
            schemaKey = apiSchema.Reference.Id.EnsureFirstCharacterToUpper();
        }
        else if (apiSchema.IsTypeArray() &&
                 apiSchema.Items?.Reference?.Id is not null)
        {
            schemaKey = apiSchema.Items.Reference.Id.EnsureFirstCharacterToUpper();
            apiSchema = apiSchema.Items;
        }
        else if (apiSchema.OneOf.Any() &&
                 apiSchema.OneOf.First().Reference?.Id is not null)
        {
            schemaKey = apiSchema.OneOf.First().Reference.Id.EnsureFirstCharacterToUpper();
            apiSchema = apiSchema.OneOf.First();
        }
        else if (apiSchema.AllOf.Count == 2 &&
                 (NameConstants.Pagination.Equals(apiSchema.AllOf[0].Reference?.Id, StringComparison.OrdinalIgnoreCase) ||
                  NameConstants.Pagination.Equals(apiSchema.AllOf[1].Reference?.Id, StringComparison.OrdinalIgnoreCase)))
        {
            if (!NameConstants.Pagination.Equals(apiSchema.AllOf[0].Reference?.Id, StringComparison.OrdinalIgnoreCase))
            {
                schemaKey = apiSchema.AllOf[0].GetModelName();
            }

            if (!NameConstants.Pagination.Equals(apiSchema.AllOf[1].Reference?.Id, StringComparison.OrdinalIgnoreCase))
            {
                schemaKey = apiSchema.AllOf[1].GetModelName();
            }
        }

        return (schemaKey, apiSchema);
    }

    private static void SetIsShared(
        List<ApiOperationSchemaMap> result)
    {
        foreach (var schemaMap in result)
        {
            var mapsForSchemaKey = result.Where(x => x.SchemaKey == schemaMap.SchemaKey).ToList();

            var segmentNames = new List<string>();
            foreach (var s in mapsForSchemaKey
                         .Select(map => map.SegmentName)
                         .Where(s => !segmentNames.Contains(s, StringComparer.Ordinal)))
            {
                segmentNames.Add(s);
            }

            if (segmentNames.Count > 1)
            {
                schemaMap.IsShared = true;
            }
        }
    }
}