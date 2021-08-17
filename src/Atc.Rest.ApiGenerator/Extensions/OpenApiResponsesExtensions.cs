using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Atc.Rest.ApiGenerator.Extensions;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;

// ReSharper disable ReturnTypeCanBeEnumerable.Global
// ReSharper disable UseDeconstruction
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable once CheckNamespace
namespace Microsoft.OpenApi.Models
{
    internal static class OpenApiResponsesExtensions
    {
        public static List<string> GetProducesResponseAttributeParts(
            this OpenApiResponses responses,
            List<ApiOperationSchemaMap> apiOperationSchemaMappings,
            string contractArea,
            string projectName,
            bool useProblemDetailsAsDefaultResponseBody,
            bool includeIfNotDefinedValidation,
            bool includeIfNotDefinedAuthorization,
            bool includeIfNotDefinedInternalServerError)
        {
            var responseTypes = GetResponseTypes(
                responses,
                apiOperationSchemaMappings,
                contractArea,
                projectName,
                useProblemDetailsAsDefaultResponseBody,
                includeEmptyResponseTypes: true,
                includeIfNotDefinedValidation,
                includeIfNotDefinedAuthorization,
                includeIfNotDefinedInternalServerError,
                isClient: false);

            return responseTypes
                .OrderBy(x => x.Item1)
                .Select(x => string.IsNullOrEmpty(x.Item2)
                    ? $"ProducesResponseType(StatusCodes.Status{(int)x.Item1}{x.Item1})"
                    : $"ProducesResponseType(typeof({x.Item2}), StatusCodes.Status{(int)x.Item1}{x.Item1})")
                .ToList();
        }

        public static List<Tuple<HttpStatusCode, string>> GetResponseTypes(
            this OpenApiResponses responses,
            List<ApiOperationSchemaMap> apiOperationSchemaMappings,
            string contractArea,
            string projectName,
            bool useProblemDetailsAsDefaultResponseBody,
            bool includeEmptyResponseTypes,
            bool includeIfNotDefinedValidation,
            bool includeIfNotDefinedAuthorization,
            bool includeIfNotDefinedInternalServerError,
            bool isClient)
        {
            var result = new List<Tuple<HttpStatusCode, string>>();
            foreach (var response in responses.OrderBy(x => x.Key))
            {
                if (!Enum.TryParse(typeof(HttpStatusCode), response.Key, out var parsedType))
                {
                    continue;
                }

                var httpStatusCode = parsedType is HttpStatusCode code ? code : 0;

                var isList = responses.IsSchemaTypeArrayForStatusCode(httpStatusCode);
                var modelName = responses.GetModelNameForStatusCode(httpStatusCode);

                var isShared = apiOperationSchemaMappings.IsShared(modelName);
                modelName = OpenApiDocumentSchemaModelNameHelper.EnsureModelNameWithNamespaceIfNeeded(projectName, contractArea, modelName, isShared, isClient);

                var useProblemDetails = responses.IsSchemaTypeProblemDetailsForStatusCode(httpStatusCode);
                if (!useProblemDetails && useProblemDetailsAsDefaultResponseBody)
                {
                    useProblemDetails = true;
                }

                string? typeResponseName;
                switch (httpStatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Created:
                        var dataType = modelName;

                        var useBinaryResponse = responses.IsSchemaUsingBinaryFormatForOkResponse();
                        if (useBinaryResponse)
                        {
                            typeResponseName = "byte[]";
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(modelName))
                            {
                                var schema = responses.GetSchemaForStatusCode(httpStatusCode);
                                if (schema != null && schema.IsSimpleDataType())
                                {
                                    dataType = schema.GetDataType();
                                }
                                else
                                {
                                    dataType = "string";
                                }
                            }

                            if (isList)
                            {
                                typeResponseName = $"{NameConstants.List}<{dataType}>";
                            }
                            else
                            {
                                var isPagination = responses.IsSchemaTypePaginationForStatusCode(httpStatusCode);
                                typeResponseName = isPagination
                                    ? $"{NameConstants.Pagination}<{dataType}>"
                                    : dataType;
                            }
                        }

                        break;
                    case HttpStatusCode.Accepted:
                    case HttpStatusCode.NoContent:
                    case HttpStatusCode.NotModified:
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                        typeResponseName = useProblemDetails
                            ? "ProblemDetails"
                            : null;
                        break;
                    case HttpStatusCode.BadRequest:
                        typeResponseName = useProblemDetails
                            ? "ValidationProblemDetails"
                            : null;
                        break;
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.MethodNotAllowed:
                    case HttpStatusCode.Conflict:
                    case HttpStatusCode.InternalServerError:
                    case HttpStatusCode.NotImplemented:
                    case HttpStatusCode.BadGateway:
                    case HttpStatusCode.ServiceUnavailable:
                    case HttpStatusCode.GatewayTimeout:
                        typeResponseName = useProblemDetails
                            ? "ProblemDetails"
                            : "string";
                        break;
                    default:
                        throw new NotImplementedException($"ResponseType for {(int)httpStatusCode} - {httpStatusCode} is missing.");
                }

                if (typeResponseName != null)
                {
                    result.Add(Tuple.Create(httpStatusCode, typeResponseName));
                }
                else if (includeEmptyResponseTypes)
                {
                    result.Add(Tuple.Create(httpStatusCode, string.Empty));
                }
            }

            if (includeIfNotDefinedValidation && result.All(x => x.Item1 != HttpStatusCode.BadRequest))
            {
                result.Add(useProblemDetailsAsDefaultResponseBody
                    ? Tuple.Create(HttpStatusCode.BadRequest, "ValidationProblemDetails")
                    : Tuple.Create(HttpStatusCode.BadRequest, string.Empty));
            }

            if (includeIfNotDefinedAuthorization)
            {
                if (result.All(x => x.Item1 != HttpStatusCode.Unauthorized))
                {
                    result.Add(useProblemDetailsAsDefaultResponseBody
                        ? Tuple.Create(HttpStatusCode.Unauthorized, "ProblemDetails")
                        : Tuple.Create(HttpStatusCode.Unauthorized, string.Empty));
                }

                if (result.All(x => x.Item1 != HttpStatusCode.Forbidden))
                {
                    result.Add(useProblemDetailsAsDefaultResponseBody
                        ? Tuple.Create(HttpStatusCode.Forbidden, "ProblemDetails")
                        : Tuple.Create(HttpStatusCode.Forbidden, string.Empty));
                }
            }

            if (includeIfNotDefinedInternalServerError && result.All(x => x.Item1 != HttpStatusCode.InternalServerError))
            {
                result.Add(useProblemDetailsAsDefaultResponseBody
                    ? Tuple.Create(HttpStatusCode.InternalServerError, "ProblemDetails")
                    : Tuple.Create(HttpStatusCode.InternalServerError, "string"));
            }

            return result;
        }
    }
}