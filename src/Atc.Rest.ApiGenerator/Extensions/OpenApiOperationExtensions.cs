using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace Microsoft.OpenApi.Models
{
    public static class OpenApiOperationExtensions
    {
        [ExcludeFromCodeCoverage]
        public static bool HasRequestBodyAnyOfFormatTypeBinary(this OpenApiOperation operation)
        {
            var schema = operation.RequestBody?.Content?.GetSchemaByFirstMediaType();
            return schema is not null &&
                   (schema.IsFormatTypeOfBinary() ||
                    schema.IsItemsOfFormatTypeBinary() ||
                    schema.HasAnyPropertiesFormatTypeBinary() ||
                    schema.HasAnyPropertiesOfArrayWithFormatTypeBinary());
        }
    }
}