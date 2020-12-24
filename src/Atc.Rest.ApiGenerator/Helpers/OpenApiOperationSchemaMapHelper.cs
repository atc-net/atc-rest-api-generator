using System;
using System.Collections.Generic;
using System.Linq;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

// ReSharper disable UseDeconstruction
// ReSharper disable InvertIf
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class OpenApiOperationSchemaMapHelper
    {
        public static string GetSegmentName(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return path
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .First()
                .PascalCase(true);
        }

        public static List<ApiOperationSchemaMap> CollectMappings(OpenApiDocument apiDocument)
        {
            if (apiDocument == null)
            {
                throw new ArgumentNullException(nameof(apiDocument));
            }

            var componentsSchemas = apiDocument.Components.Schemas;

            var list = new List<ApiOperationSchemaMap>();
            foreach (var apiPath in apiDocument.Paths)
            {
                foreach (var apiOperation in apiPath.Value.Operations)
                {
                    // Parameters
                    CollectMappingsForParameters(componentsSchemas, apiOperation, apiPath, list);

                    // RequestBody
                    CollectMappingsForRequestBody(componentsSchemas, apiOperation, apiPath, list);

                    // Responses
                    CollectMappingsForResponses(componentsSchemas, apiOperation, apiPath, list);
                }
            }

            return list;
        }

        private static void CollectMappingsForParameters(
            IDictionary<string, OpenApiSchema> componentsSchemas,
            KeyValuePair<OperationType, OpenApiOperation> apiOperation,
            KeyValuePair<string, OpenApiPathItem> apiPath,
            List<ApiOperationSchemaMap> list)
        {
            foreach (var apiParameter in apiOperation.Value.Parameters)
            {
                if (apiParameter.Content == null)
                {
                    continue;
                }

                foreach (var apiMediaType in apiParameter.Content)
                {
                    CollectSchema(
                        componentsSchemas,
                        apiMediaType.Value.Schema,
                        SchemaMapLocatedAreaType.Parameter,
                        apiPath.Key,
                        apiOperation.Key,
                        null,
                        list);
                }
            }
        }

        private static void CollectMappingsForRequestBody(
            IDictionary<string, OpenApiSchema> componentsSchemas,
            KeyValuePair<OperationType, OpenApiOperation> apiOperation,
            KeyValuePair<string, OpenApiPathItem> apiPath,
            List<ApiOperationSchemaMap> list)
        {
            if (apiOperation.Value.RequestBody?.Content != null)
            {
                foreach (var apiMediaType in apiOperation.Value.RequestBody.Content)
                {
                    CollectSchema(
                        componentsSchemas,
                        apiMediaType.Value.Schema,
                        SchemaMapLocatedAreaType.RequestBody,
                        apiPath.Key,
                        apiOperation.Key,
                        null,
                        list);
                }
            }
        }

        private static void CollectMappingsForResponses(
            IDictionary<string, OpenApiSchema> componentsSchemas,
            KeyValuePair<OperationType, OpenApiOperation> apiOperation,
            KeyValuePair<string, OpenApiPathItem> apiPath,
            List<ApiOperationSchemaMap> list)
        {
            foreach (var apiResponse in apiOperation.Value.Responses)
            {
                if (apiResponse.Value?.Content == null)
                {
                    continue;
                }

                foreach (var apiMediaType in apiResponse.Value.Content)
                {
                    CollectSchema(
                        componentsSchemas,
                        apiMediaType.Value.Schema,
                        SchemaMapLocatedAreaType.Response,
                        apiPath.Key,
                        apiOperation.Key,
                        null,
                        list);
                }
            }
        }

        private static void CollectSchema(
            IDictionary<string, OpenApiSchema> componentsSchemas,
            OpenApiSchema? apiSchema,
            SchemaMapLocatedAreaType locatedArea,
            string apiPath,
            OperationType apiOperationType,
            string? parentApiSchema,
            List<ApiOperationSchemaMap> list)
        {
            if (apiSchema == null)
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

                if (subSchemaKey != null)
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
        }

        private static void Collect(
            IDictionary<string, OpenApiSchema> componentsSchemas,
            IEnumerable<KeyValuePair<string, OpenApiSchema>> propertySchemas,
            SchemaMapLocatedAreaType areaType,
            string apiPath,
            OperationType apiOperationType,
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

        private static (string, OpenApiSchema) ConsolidateSchemaObjectTypes(OpenApiSchema apiSchema)
        {
            var schemaKey = string.Empty;
            if (apiSchema.Reference?.Id != null)
            {
                schemaKey = apiSchema.Reference.Id.EnsureFirstCharacterToUpper();
            }
            else if (apiSchema.Type == OpenApiDataTypeConstants.Array &&
                     apiSchema.Items?.Reference?.Id != null)
            {
                schemaKey = apiSchema.Items.Reference.Id.EnsureFirstCharacterToUpper();
                apiSchema = apiSchema.Items;
            }
            else if (apiSchema.OneOf.Any() && apiSchema.OneOf.First().Reference?.Id != null)
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
}