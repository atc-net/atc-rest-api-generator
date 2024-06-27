// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
// ReSharper disable TailRecursiveCall
namespace Atc.Rest.ApiGenerator.OpenApi.Extractors;

public sealed class ApiOperationExtractor : IApiOperationExtractor
{
    public static readonly char[] ModelNameSeparators = { ' ', '-', '_', '.' };

    public IList<ApiOperation> Extract(
        OpenApiDocument apiDocument)
    {
        ArgumentNullException.ThrowIfNull(apiDocument);

        var componentsSchemas = apiDocument.Components.Schemas;

        var result = new List<ApiOperation>();
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
        List<ApiOperation> result)
    {
        foreach (var apiParameter in apiOperation.OpenApiOperation.Parameters)
        {
            if (apiParameter.Content is null)
            {
                continue;
            }

            if (apiParameter.Content.Keys.Count > 0)
            {
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
            else
            {
                CollectSchema(
                    componentsSchemas,
                    apiSchema: apiParameter.Schema,
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
        List<ApiOperation> result)
    {
        if (apiOperation.OpenApiOperation.RequestBody?.Content is null)
        {
            return;
        }

        if (apiOperation.OpenApiOperation.RequestBody.Content.Count > 0)
        {
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
    }

    private static void ExtractMappingsFromResponses(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        (HttpOperationType HttpOperationType, OpenApiOperation OpenApiOperation) apiOperation,
        KeyValuePair<string, OpenApiPathItem> apiPath,
        List<ApiOperation> result)
    {
        foreach (var apiResponse in apiOperation.OpenApiOperation.Responses)
        {
            if (apiResponse.Value?.Content is null)
            {
                continue;
            }

            if (apiResponse.Value.Content.Keys.Count > 0)
            {
                foreach (var apiMediaType in apiResponse.Value.Content.Where(x => x.Key.Equals(MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase)))
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
    }

    private static void CollectSchema(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        OpenApiSchema? apiSchema,
        ApiSchemaMapLocatedAreaType locatedArea,
        string apiPath,
        HttpOperationType httpOperation,
        string? parentApiSchema,
        List<ApiOperation> result)
    {
        if (apiSchema is null)
        {
            return;
        }

        if (apiSchema.IsTypeCustomPagination())
        {
            var apiSchemaForCustomPagination = apiSchema.GetCustomPaginationSchema();
            var apiSchemaForCustomPaginationItems = apiSchema.GetCustomPaginationItemsSchema();
            if (apiSchemaForCustomPagination is not null &&
                apiSchemaForCustomPaginationItems is not null)
            {
                var consolidateSchemaForCustomPaginationItems = ConsolidateSchemaObjectTypes(apiSchemaForCustomPaginationItems);
                CollectSchema(
                    componentsSchemas,
                    consolidateSchemaForCustomPaginationItems.Item2,
                    locatedArea,
                    apiPath,
                    httpOperation,
                    parentApiSchema,
                    result);

                var consolidateSchemaForCustomPagination = ConsolidateSchemaObjectTypes(apiSchemaForCustomPagination);
                CollectSchema(
                    componentsSchemas,
                    consolidateSchemaForCustomPagination.Item2,
                    locatedArea,
                    apiPath,
                    httpOperation,
                    parentApiSchema,
                    result);
            }

            return;
        }

        (var schemaKey, apiSchema) = ConsolidateSchemaObjectTypes(apiSchema);

        if (string.IsNullOrEmpty(schemaKey) ||
            schemaKey == "ProblemDetails" ||
            schemaKey.Equals(NameConstants.Pagination, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var apiOperation = new ApiOperation(schemaKey.PascalCase(), locatedArea, apiPath, httpOperation, parentApiSchema);
        if (result.Exists(x => x.Equals(apiOperation)))
        {
            return;
        }

        if (apiSchema.IsSchemaEnumOrPropertyEnum())
        {
            apiOperation.Model.IsEnum = true;
        }

        if (apiSchema.HasAnyPropertiesWithFormatTypeBinary())
        {
            apiOperation.Model.UsesIFormFile = true;
        }

        if (apiOperation.Cardinality == CardinalityType.None)
        {
            apiOperation.Cardinality = CardinalityType.Single;
        }

        result.Add(apiOperation);

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
        else if (apiSchema.IsTypePagination())
        {
            apiOperation.Cardinality = CardinalityType.Paged;
            HandleCardinalityPaged(componentsSchemas, apiSchema, locatedArea, apiPath, httpOperation, result, schemaKey);
        }
        else if (apiSchema.IsArray())
        {
            apiOperation.Cardinality = CardinalityType.Multiple;
            HandleCardinalityMultiple(componentsSchemas, apiSchema, locatedArea, apiPath, httpOperation, parentApiSchema, result);
        }
    }

    private static void HandleCardinalityMultiple(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        OpenApiSchema apiSchema,
        ApiSchemaMapLocatedAreaType locatedArea,
        string apiPath,
        HttpOperationType httpOperation,
        string? parentApiSchema,
        List<ApiOperation> result)
    {
        var subSchemaKey = apiSchema.Items.GetModelName();
        var subApiOperation = new ApiOperation(subSchemaKey, locatedArea, apiPath, httpOperation, parentApiSchema);
        if (result.Any(x => x.Equals(subApiOperation)))
        {
            return;
        }

        if (subApiOperation.Cardinality == CardinalityType.None)
        {
            subApiOperation.Cardinality = CardinalityType.Single;
        }

        result.Add(subApiOperation);

        var subApiSchema = componentsSchemas.Single(x => x.GetFormattedKey().Equals(subSchemaKey, StringComparison.OrdinalIgnoreCase)).Value;
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

    private static void HandleCardinalityPaged(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        OpenApiSchema apiSchema,
        ApiSchemaMapLocatedAreaType locatedArea,
        string apiPath,
        HttpOperationType httpOperation,
        List<ApiOperation> result,
        string schemaKey)
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

        var subApiSchema = componentsSchemas.Single(x => x.GetFormattedKey().Equals(schemaKey, StringComparison.OrdinalIgnoreCase)).Value;

        Collect(
            componentsSchemas,
            subApiSchema.Properties.ToList(),
            locatedArea,
            apiPath,
            httpOperation,
            subSchemaKey,
            result);
    }

    private static void Collect(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        IEnumerable<KeyValuePair<string, OpenApiSchema>> propertySchemas,
        ApiSchemaMapLocatedAreaType locatedArea,
        string apiPath,
        HttpOperationType httpOperation,
        string parentApiSchema,
        List<ApiOperation> result)
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
            schemaKey = apiSchema.Reference.Id.PascalCase(ModelNameSeparators, removeSeparators: true);
        }
        else if (apiSchema.IsArray())
        {
            schemaKey = apiSchema.Items.Reference.Id.PascalCase(ModelNameSeparators, removeSeparators: true);
            apiSchema = apiSchema.Items;
        }
        else if (apiSchema.OneOf.Any() &&
                 apiSchema.OneOf[0].Reference?.Id is not null)
        {
            schemaKey = apiSchema.OneOf[0].Reference.Id.PascalCase(ModelNameSeparators, removeSeparators: true);
            apiSchema = apiSchema.OneOf[0];
        }
        else if (apiSchema.AnyOf.Any() &&
                 apiSchema.AnyOf[0].Reference?.Id is not null)
        {
            schemaKey = apiSchema.AnyOf[0].Reference.Id.PascalCase(ModelNameSeparators, removeSeparators: true);
            apiSchema = apiSchema.AnyOf[0];
        }
        else if (apiSchema.AllOf.Any() &&
                 apiSchema.AllOf[0].Reference?.Id is not null)
        {
            schemaKey = apiSchema.AllOf[0].Reference.Id.PascalCase(ModelNameSeparators, removeSeparators: true);
            apiSchema = apiSchema.AllOf[0];
        }
        else if (apiSchema.IsTypePagination())
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
        List<ApiOperation> result)
    {
        foreach (var schemaMap in result)
        {
            var mapsForSchemaKey = result.Where(x => x.Model.Name == schemaMap.Model.Name).ToList();

            var apiGroupNames = new List<string>();
            foreach (var s in mapsForSchemaKey
                         .Select(map => map.ApiGroupName)
                         .Where(s => !apiGroupNames.Contains(s, StringComparer.Ordinal)))
            {
                apiGroupNames.Add(s);
            }

            if (apiGroupNames.Count > 1)
            {
                schemaMap.Model.IsShared = true;
            }
        }
    }
}