using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.Extensions
{
    public static class OpenApiOperationExtensions
    {
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