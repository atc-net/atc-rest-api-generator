// ReSharper disable UseDeconstruction
// ReSharper disable InvertIf
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

namespace Atc.Rest.ApiGenerator.Helpers;

public static class OpenApiOperationSchemaMapHelper
{
    public static List<ApiOperationSchemaMap> CollectMappings(
        OpenApiDocument apiDocument)
    {
        ArgumentNullException.ThrowIfNull(apiDocument);

        var componentsSchemas = apiDocument.Components.Schemas;

        var list = new List<ApiOperationSchemaMap>();
        foreach (var apiPath in apiDocument.Paths)
        {
            foreach (var apiOperation in apiPath.Value.Operations)
            {
                // Parameters
                CollectMappingsForParameters(componentsSchemas, (apiOperation.Key.ToHttpOperationType(), apiOperation.Value), apiPath, list);

                // RequestBody
                CollectMappingsForRequestBody(componentsSchemas, (apiOperation.Key.ToHttpOperationType(), apiOperation.Value), apiPath, list);

                // Responses
                CollectMappingsForResponses(componentsSchemas, (apiOperation.Key.ToHttpOperationType(), apiOperation.Value), apiPath, list);
            }
        }

        return list;
    }

    private static void CollectMappingsForParameters(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        (HttpOperationType HttpOperationType, OpenApiOperation OpenApiOperation) apiOperation,
        KeyValuePair<string, OpenApiPathItem> apiPath,
        List<ApiOperationSchemaMap> list)
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
                    apiMediaType.Value.Schema,
                    ApiSchemaMapLocatedAreaType.Parameter,
                    apiPath.Key,
                    apiOperation.HttpOperationType,
                    null,
                    list);
            }
        }
    }

    private static void CollectMappingsForRequestBody(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        (HttpOperationType HttpOperationType, OpenApiOperation OpenApiOperation) apiOperation,
        KeyValuePair<string, OpenApiPathItem> apiPath,
        List<ApiOperationSchemaMap> list)
    {
        if (apiOperation.OpenApiOperation.RequestBody?.Content is not null)
        {
            foreach (var apiMediaType in apiOperation.OpenApiOperation.RequestBody.Content)
            {
                CollectSchema(
                    componentsSchemas,
                    apiMediaType.Value.Schema,
                    ApiSchemaMapLocatedAreaType.RequestBody,
                    apiPath.Key,
                    apiOperation.HttpOperationType,
                    null,
                    list);
            }
        }
    }

    private static void CollectMappingsForResponses(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        (HttpOperationType HttpOperationType, OpenApiOperation OpenApiOperation) apiOperation,
        KeyValuePair<string, OpenApiPathItem> apiPath,
        List<ApiOperationSchemaMap> list)
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
                    apiMediaType.Value.Schema,
                    ApiSchemaMapLocatedAreaType.Response,
                    apiPath.Key,
                    apiOperation.HttpOperationType,
                    null,
                    list);
            }
        }
    }

    private static void CollectSchema(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        OpenApiSchema? apiSchema,
        ApiSchemaMapLocatedAreaType locatedArea,
        string apiPath,
        HttpOperationType apiOperationType,
        string? parentApiSchema,
        List<ApiOperationSchemaMap> list)
    {
        if (apiSchema is null)
        {
            return;
        }

        string schemaKey;
        (schemaKey, apiSchema) = ConsolidateSchemaObjectTypes(apiSchema);

        if (schemaKey.Length == 0 ||
            schemaKey == nameof(ProblemDetails) ||
            schemaKey.Equals(Microsoft.OpenApi.Models.NameConstants.Pagination, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var apiOperationSchemaMap = new ApiOperationSchemaMap(schemaKey, locatedArea, apiPath, apiOperationType, parentApiSchema);
        if (list.Any(x => x.Equals(apiOperationSchemaMap)))
        {
            return;
        }

        list.Add(apiOperationSchemaMap);
        if (apiSchema.Properties.Any())
        {
            Collect(
                componentsSchemas,
                apiSchema.Properties.ToList(),
                locatedArea,
                apiPath,
                apiOperationType,
                schemaKey,
                list);
        }
        else if (apiSchema.AllOf.Count == 2 &&
                 (Microsoft.OpenApi.Models.NameConstants.Pagination.Equals(apiSchema.AllOf[0].Reference?.Id, StringComparison.OrdinalIgnoreCase) ||
                  Microsoft.OpenApi.Models.NameConstants.Pagination.Equals(apiSchema.AllOf[1].Reference?.Id, StringComparison.OrdinalIgnoreCase)))
        {
            string? subSchemaKey = null;
            if (!Microsoft.OpenApi.Models.NameConstants.Pagination.Equals(apiSchema.AllOf[0].Reference?.Id, StringComparison.OrdinalIgnoreCase))
            {
                subSchemaKey = apiSchema.AllOf[0].GetModelName();
            }

            if (!Microsoft.OpenApi.Models.NameConstants.Pagination.Equals(apiSchema.AllOf[1].Reference?.Id, StringComparison.OrdinalIgnoreCase))
            {
                subSchemaKey = apiSchema.AllOf[1].GetModelName();
            }

            if (subSchemaKey is not null)
            {
                var subApiSchema = componentsSchemas.Single(x => x.Key.Equals(schemaKey, StringComparison.OrdinalIgnoreCase)).Value;
                Collect(
                    componentsSchemas,
                    subApiSchema.Properties.ToList(),
                    locatedArea,
                    apiPath,
                    apiOperationType,
                    subSchemaKey,
                    list);
            }
        }
        else if (apiSchema.IsTypeArray() &&
                 apiSchema.Items?.Reference?.Id is not null)
        {
            var subSchemaKey = apiSchema.Items.GetModelName();
            var subApiOperationSchemaMap = new ApiOperationSchemaMap(subSchemaKey, locatedArea, apiPath, apiOperationType, parentApiSchema);
            if (!list.Any(x => x.Equals(subApiOperationSchemaMap)))
            {
                list.Add(subApiOperationSchemaMap);

                var subApiSchema = componentsSchemas.Single(x => x.Key.Equals(subSchemaKey, StringComparison.OrdinalIgnoreCase)).Value;
                if (subApiSchema.Properties.Any())
                {
                    Collect(
                        componentsSchemas,
                        subApiSchema.Properties.ToList(),
                        locatedArea,
                        apiPath,
                        apiOperationType,
                        subSchemaKey,
                        list);
                }
            }
        }
    }

    private static void Collect(
        IDictionary<string, OpenApiSchema> componentsSchemas,
        IEnumerable<KeyValuePair<string, OpenApiSchema>> propertySchemas,
        ApiSchemaMapLocatedAreaType areaType,
        string apiPath,
        HttpOperationType apiOperationType,
        string parentApiSchema,
        List<ApiOperationSchemaMap> list)
    {
        foreach (var propertySchema in propertySchemas)
        {
            CollectSchema(
                componentsSchemas,
                propertySchema.Value,
                areaType,
                apiPath,
                apiOperationType,
                parentApiSchema,
                list);
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
                 (Microsoft.OpenApi.Models.NameConstants.Pagination.Equals(apiSchema.AllOf[0].Reference?.Id, StringComparison.OrdinalIgnoreCase) ||
                  Microsoft.OpenApi.Models.NameConstants.Pagination.Equals(apiSchema.AllOf[1].Reference?.Id, StringComparison.OrdinalIgnoreCase)))
        {
            if (!Microsoft.OpenApi.Models.NameConstants.Pagination.Equals(apiSchema.AllOf[0].Reference?.Id, StringComparison.OrdinalIgnoreCase))
            {
                schemaKey = apiSchema.AllOf[0].GetModelName();
            }

            if (!Microsoft.OpenApi.Models.NameConstants.Pagination.Equals(apiSchema.AllOf[1].Reference?.Id, StringComparison.OrdinalIgnoreCase))
            {
                schemaKey = apiSchema.AllOf[1].GetModelName();
            }
        }

        return (schemaKey, apiSchema);
    }
}