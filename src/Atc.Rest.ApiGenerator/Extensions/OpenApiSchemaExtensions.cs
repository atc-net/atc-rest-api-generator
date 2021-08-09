using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Atc.Rest.ApiGenerator.Extensions;
using Atc.Rest.ApiGenerator.Models;

// ReSharper disable once CheckNamespace
namespace Microsoft.OpenApi.Models
{
    // TODO: Move to ATC.OpenApi
    [ExcludeFromCodeCoverage]
    public static class OpenApiSchemaExtensions
    {
        public static bool HasAnyPropertiesFormatTypeFromSystemNamespace(this OpenApiSchema schema)
        {
            return schema.HasAnyProperties() &&
                   schema.Properties.Any(x => x.Value.HasFormatTypeFromSystemNamespace());
        }

        public static bool HasAnyPropertiesFormatTypeFromSystemNamespace(this OpenApiSchema schema, IDictionary<string, OpenApiSchema> componentsSchemas)
        {
            if (!schema.HasAnyProperties())
            {
                return false;
            }

            foreach (var schemaProperty in schema.Properties)
            {
                if (schemaProperty.Value.HasFormatTypeFromSystemNamespace())
                {
                    return true;
                }

                if (!schemaProperty.Value.IsObjectReferenceTypeDeclared())
                {
                    continue;
                }

                var childModelName = schemaProperty.Value.GetModelName();
                if (string.IsNullOrEmpty(childModelName))
                {
                    continue;
                }

                var childSchema = componentsSchemas.FirstOrDefault(x => x.Key == childModelName);
                if (string.IsNullOrEmpty(childSchema.Key))
                {
                    continue;
                }

                if (childSchema.Value.HasAnyPropertiesFormatTypeFromSystemNamespace(componentsSchemas))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasAnyPropertiesFormatFromSystemCollectionGenericNamespace(this OpenApiSchema schema, IDictionary<string, OpenApiSchema> componentsSchemas)
        {
            if (!schema.HasAnyProperties())
            {
                return false;
            }

            foreach (var schemaProperty in schema.Properties)
            {
                if (schemaProperty.Value.HasDataTypeFromSystemCollectionGenericNamespace())
                {
                    return true;
                }

                if (!schemaProperty.Value.IsObjectReferenceTypeDeclared())
                {
                    continue;
                }

                var childModelName = schemaProperty.Value.GetModelName();
                if (string.IsNullOrEmpty(childModelName))
                {
                    continue;
                }

                var childSchema = componentsSchemas.FirstOrDefault(x => x.Key == childModelName);
                if (string.IsNullOrEmpty(childSchema.Key))
                {
                    continue;
                }

                if (childSchema.Value.HasAnyPropertiesFormatFromSystemCollectionGenericNamespace(componentsSchemas))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsArrayReferenceTypeDeclared2(this OpenApiSchema schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            return schema.Type == OpenApiDataTypeConstants.Array &&
                   schema.Items?.Reference != null;
        }

        public static bool HasAnySharedModel(this OpenApiSchema schema, List<ApiOperationSchemaMap> apiOperationSchemaMaps)
        {
            if (!schema.IsObjectReferenceTypeDeclared())
            {
                return false;
            }

            var modelName = schema.GetModelName();
            if (apiOperationSchemaMaps.IsShared(modelName))
            {
                return true;
            }

            return schema.HasAnyProperties() &&
                   schema.Properties.Any(x => HasAnySharedModel(x.Value, apiOperationSchemaMaps));
        }

        public static bool HasAnySharedModelOrEnum(this OpenApiSchema schema, List<ApiOperationSchemaMap> apiOperationSchemaMaps, bool includeProperties = true)
        {
            if (!schema.IsObjectReferenceTypeDeclared())
            {
                return false;
            }

            if (schema.IsSchemaEnumOrPropertyEnum())
            {
                return true;
            }

            var modelName = schema.GetModelName();
            if (apiOperationSchemaMaps.IsShared(modelName))
            {
                return true;
            }

            if (!includeProperties)
            {
                return false;
            }

            return schema.HasAnyProperties() &&
                   schema.Properties.Any(x => x.Value.HasAnySharedModelOrEnum(apiOperationSchemaMaps));
        }
    }
}