using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.Models
{
    public class EndpointMethodMetadata
    {
        public EndpointMethodMetadata(
            string projectName,
            string segmentName,
            string route,
            OperationType httpOperation,
            string methodName,
            bool isSharedResponseModel,
            string contractInterfaceHandlerTypeName,
            string? contractParameterTypeName,
            string? contractResultTypeName,
            List<Tuple<HttpStatusCode, string, OpenApiSchema?>> contractReturnTypeNames,
            SyntaxGeneratorContractParameter? sgContractParameter,
            IDictionary<string, OpenApiSchema> componentsSchemas)
        {
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
        }

        public string ProjectName { get; private set; }

        public string SegmentName { get; private set; }

        public string Route { get; private set; }

        public OperationType HttpOperation { get; private set; }

        public string MethodName { get; private set; }

        public bool IsSharedResponseModel { get; private set; }

        public string ContractInterfaceHandlerTypeName { get; private set; }

        public string? ContractParameterTypeName { get; private set; }

        public string? ContractResultTypeName { get; private set; }

        public List<Tuple<HttpStatusCode, string, OpenApiSchema?>> ContractReturnTypeNames { get; private set; }

        public SyntaxGeneratorContractParameter? ContractParameter { get; private set; }

        public IDictionary<string, OpenApiSchema> ComponentsSchemas { get; private set; }

        public bool IsPaginationUsed()
        {
            var returnType = ContractReturnTypeNames.FirstOrDefault(x => x.Item1 == HttpStatusCode.OK)?.Item2;
            return returnType != null &&
                   returnType.StartsWith(Microsoft.OpenApi.Models.NameConstants.Pagination, StringComparison.Ordinal);
        }

        public bool HasContractParameterRequestBody()
        {
            return ContractParameter?.ApiOperation.RequestBody?.Content.GetSchema() != null;
        }

        public bool HasContractParameterRequiredHeader()
        {
            return GetHeaderRequiredParameters().Count > 0;
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

        public override string ToString()
        {
            return $"{nameof(SegmentName)}: {SegmentName}, {nameof(HttpOperation)}: {HttpOperation}, {nameof(MethodName)}: {MethodName}, {nameof(Route)}: {Route}";
        }
    }
}