using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Atc.Rest.ApiGenerator.Extensions;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.SyntaxGenerators;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.Models
{
    public class EndpointMethodMetadata
    {
        public EndpointMethodMetadata(
            bool useNullableReferenceTypes,
            string projectName,
            string segmentName,
            string route,
            OperationType httpOperation,
            string methodName,
            bool isSharedResponseModel,
            string contractInterfaceHandlerTypeName,
            string? contractParameterTypeName,
            string? contractResultTypeName,
            List<ResponseTypeNameAndItemSchema> contractReturnTypeNames,
            SyntaxGeneratorContractParameter? sgContractParameter,
            IDictionary<string, OpenApiSchema> componentsSchemas,
            List<ApiOperationSchemaMap> apiOperationSchemaMappings)
        {
            UseNullableReferenceTypes = useNullableReferenceTypes;
            ProjectName = projectName;
            SegmentName = segmentName;
            Route = route;
            HttpOperation = httpOperation;
            MethodName = methodName;
            IsSharedResponseModel = isSharedResponseModel;
            ContractInterfaceHandlerTypeName = contractInterfaceHandlerTypeName;
            ContractParameterTypeName = contractParameterTypeName;
            ContractResultTypeName = contractResultTypeName;
            ContractReturnTypeNames = contractReturnTypeNames;
            ContractParameter = sgContractParameter;
            ComponentsSchemas = componentsSchemas;
            OperationSchemaMappings = apiOperationSchemaMappings;
        }

        public bool UseNullableReferenceTypes { get; private set; }

        public string ProjectName { get; private set; }

        public string SegmentName { get; private set; }

        public string Route { get; private set; }

        public OperationType HttpOperation { get; private set; }

        public string MethodName { get; private set; }

        public bool IsSharedResponseModel { get; private set; }

        public string ContractInterfaceHandlerTypeName { get; private set; }

        public string? ContractParameterTypeName { get; private set; }

        public string? ContractResultTypeName { get; private set; }

        public List<ResponseTypeNameAndItemSchema> ContractReturnTypeNames { get; private set; }

        public SyntaxGeneratorContractParameter? ContractParameter { get; private set; }

        public IDictionary<string, OpenApiSchema> ComponentsSchemas { get; private set; }

        private List<ApiOperationSchemaMap> OperationSchemaMappings { get; }

        public bool IsContractReturnTypeUsingPagination()
        {
            var returnType = ContractReturnTypeNames.FirstOrDefault(x => x.StatusCode == HttpStatusCode.OK)?.FullModelName;
            return !string.IsNullOrEmpty(returnType) &&
                   returnType.StartsWith(Microsoft.OpenApi.Models.NameConstants.Pagination, StringComparison.Ordinal);
        }

        public bool IsContractReturnTypeUsingList()
        {
            var returnType = ContractReturnTypeNames.FirstOrDefault(x => x.StatusCode == HttpStatusCode.OK)?.FullModelName;
            return !string.IsNullOrEmpty(returnType) &&
                   returnType.StartsWith(Microsoft.OpenApi.Models.NameConstants.List, StringComparison.Ordinal);
        }

        public bool IsContractReturnTypeUsingSystemCollectionGenericNamespace()
        {
            if (IsContractReturnTypeUsingList())
            {
                return true;
            }

            var responseType = ContractReturnTypeNames.FirstOrDefault(x => x.StatusCode == HttpStatusCode.OK);
            if (responseType is null)
            {
                return false;
            }

            return responseType.Schema is not null &&
                   responseType.Schema.HasAnyPropertiesFormatFromSystemCollectionGenericNamespace(ComponentsSchemas);
        }

        public bool IsContractReturnTypeUsingString()
        {
            var responseType = ContractReturnTypeNames.FirstOrDefault(x => x.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created);

            return responseType is not null &&
                   OpenApiDataTypeConstants.String.Equals(responseType.FullModelName, StringComparison.Ordinal);
        }

        public bool IsContractReturnTypeUsingSystemNamespace()
        {
            return ContractReturnTypeNames
                .Where(x => x.Schema != null &&
                            x.Schema.IsObjectReferenceTypeDeclared())
                .Any(x => x.Schema != null &&
                          x.Schema.HasAnyPropertiesFormatTypeFromSystemNamespace(ComponentsSchemas));
        }

        public bool IsContractParameterRequestBodyUsed()
        {
            var schema = ContractParameter?.ApiOperation.RequestBody?.Content.GetSchema();
            return schema is not null;
        }

        public bool IsContractParameterRequestBodyUsingSystemCollectionGenericNamespace()
        {
            var schema = ContractParameter?.ApiOperation.RequestBody?.Content.GetSchema();
            return schema is not null &&
                   (schema.IsArrayReferenceTypeDeclared2() ||
                   schema.HasAnyPropertiesFormatFromSystemCollectionGenericNamespace(ComponentsSchemas));
        }

        public bool IsContractParameterRequestBodyUsingSystemNamespace()
        {
            var schema = ContractParameter?.ApiOperation.RequestBody?.Content.GetSchema();
            return schema is not null &&
                   schema.HasAnyPropertiesFormatTypeFromSystemNamespace(ComponentsSchemas);
        }

        public bool IsContractParameterRequestBodyUsingStringBuilder()
        {
            var schema = ContractParameter?.ApiOperation.RequestBody?.Content.GetSchema();
            if (schema is null)
            {
                return false;
            }

            if (schema.IsArrayReferenceTypeDeclared2())
            {
                var childSchemaKey = schema.Items.GetModelName();
                var childSchema = ComponentsSchemas.FirstOrDefault(x => x.Key.Equals(childSchemaKey, StringComparison.Ordinal));

                if (childSchema.Key is not null)
                {
                    var childRelevantSchemas = GetRelevantSchemasForBadRequestBodyParameters(childSchema.Value);
                    return childRelevantSchemas.Count > 0;
                }
            }

            var relevantSchemas = GetRelevantSchemasForBadRequestBodyParameters(schema);
            return relevantSchemas.Count > 0;
        }

        public bool HasContractParameterRequestBody()
        {
            return ContractParameter?.ApiOperation.RequestBody?.Content.GetSchema() is not null;
        }

        public bool HasContractParameterRequiredHeader()
        {
            return GetHeaderRequiredParameters().Count > 0;
        }

        public bool HasContractReturnTypeAsComplexAndNotSharedModel()
        {
            var returnType = ContractReturnTypeNames.FirstOrDefault(x => x.StatusCode == HttpStatusCode.OK);
            if (returnType is null ||
                string.IsNullOrEmpty(returnType.FullModelName) ||
                returnType.Schema?.Type != OpenApiDataTypeConstants.Object)
            {
                return false;
            }

            var rawModelName = OpenApiDocumentSchemaModelNameHelper.GetRawModelName(returnType.FullModelName);
            return !OperationSchemaMappings.IsShared(rawModelName);
        }

        public bool HasContractReturnTypeNamesOnlySimpleTypes()
            => ContractReturnTypeNames.All(x => x.Schema is null);

        public bool HasSharedModelOrEnumInContractParameterRequestBody()
        {
            var schema = ContractParameter?.ApiOperation.RequestBody?.Content.GetSchema();
            return schema is not null &&
                   schema.HasAnySharedModelOrEnum(OperationSchemaMappings);
        }

        public bool HasSharedModelInContractReturnType()
        {
            foreach (var item in ContractReturnTypeNames)
            {
                if (item.Schema is null)
                {
                    continue;
                }

                if (item.Schema.HasAnySharedModelOrEnum(OperationSchemaMappings))
                {
                    return true;
                }
            }

            return false;
        }

        public List<OpenApiParameter> GetRouteParameters()
        {
            var list = new List<OpenApiParameter>();
            if (ContractParameter == null)
            {
                return list;
            }

            list.AddRange(ContractParameter.ApiOperation.Parameters.GetAllFromRoute());
            list.AddRange(ContractParameter.GlobalPathParameters.GetAllFromRoute());
            return list;
        }

        public List<OpenApiParameter> GetHeaderParameters()
        {
            return ContractParameter == null
                ? new List<OpenApiParameter>()
                : ContractParameter.ApiOperation.Parameters.GetAllFromHeader();
        }

        public List<OpenApiParameter> GetHeaderRequiredParameters()
        {
            return GetHeaderParameters()
                .Where(parameter => parameter.Required)
                .ToList();
        }

        public List<OpenApiParameter> GetQueryParameters()
        {
            return ContractParameter == null
                ? new List<OpenApiParameter>()
                : ContractParameter.ApiOperation.Parameters.GetAllFromQuery();
        }

        public List<OpenApiParameter> GetQueryRequiredParameters()
        {
            return GetQueryParameters()
                .Where(parameter => parameter.Required)
                .ToList();
        }

        public List<KeyValuePair<string, OpenApiSchema>> GetRelevantSchemasForBadRequestBodyParameters(OpenApiSchema modelSchema)
        {
            var relevantSchemas = new List<KeyValuePair<string, OpenApiSchema>>();
            foreach (var schemaProperty in modelSchema.Properties)
            {
                if (UseNullableReferenceTypes &&
                    schemaProperty.Value.Type == OpenApiDataTypeConstants.Array)
                {
                    continue;
                }

                if (modelSchema.Required.Contains(schemaProperty.Key) ||
                    schemaProperty.Value.IsFormatTypeOfEmail() ||
                    schemaProperty.Value.IsFormatTypeOfDate() ||
                    schemaProperty.Value.IsFormatTypeOfDateTime() ||
                    schemaProperty.Value.IsFormatTypeOfTime() ||
                    schemaProperty.Value.IsFormatTypeOfTimestamp())
                {
                    relevantSchemas.Add(schemaProperty);
                }
            }

            return relevantSchemas;
        }

        public bool Contains(string value)
        {
            if (ContractParameterTypeName is not null &&
                ContractParameterTypeName.Contains(value, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (ContractResultTypeName is not null &&
                ContractResultTypeName.Contains(value, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (ContractParameter is not null)
            {
                if (ContractParameter.ApiOperation.GetOperationName().Contains(value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                var requestModelName = ContractParameter.ApiOperation.GetModelSchemaFromRequest()?.GetModelName();
                if (requestModelName is not null &&
                    requestModelName.Contains(value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                var responseModelName = ContractParameter.ApiOperation.GetModelSchemaFromResponse()?.GetModelName();
                if (responseModelName is not null &&
                    responseModelName.Contains(value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return $"{nameof(SegmentName)}: {SegmentName}, {nameof(HttpOperation)}: {HttpOperation}, {nameof(MethodName)}: {MethodName}, {nameof(Route)}: {Route}";
        }
    }
}