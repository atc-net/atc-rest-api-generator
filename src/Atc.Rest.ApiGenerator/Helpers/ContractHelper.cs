using System;
using System.Collections.Generic;
using System.Net;
using Atc.Rest.ApiGenerator.Extensions;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.OpenApi.Models;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class ContractHelper
    {
        public static bool HasSharedResponseContract(
            OpenApiDocument document,
            List<ApiOperationSchemaMap> operationSchemaMappings,
            string focusOnSegmentName)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (operationSchemaMappings == null)
            {
                throw new ArgumentNullException(nameof(operationSchemaMappings));
            }

            if (focusOnSegmentName == null)
            {
                throw new ArgumentNullException(nameof(focusOnSegmentName));
            }

            foreach (var (_, value) in document.GetPathsByBasePathSegmentName(focusOnSegmentName))
            {
                foreach (var apiOperation in value.Operations)
                {
                    if (apiOperation.Value.Responses == null)
                    {
                        continue;
                    }

                    var responseModelName = apiOperation.Value.Responses.GetModelNameForStatusCode(HttpStatusCode.OK);
                    var isSharedResponseModel = !string.IsNullOrEmpty(responseModelName) &&
                                                operationSchemaMappings.IsShared(responseModelName);
                    if (isSharedResponseModel)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool HasList(string typeName)
        {
            return !string.IsNullOrEmpty(typeName) &&
                   typeName.Contains(Microsoft.OpenApi.Models.NameConstants.List + "<", StringComparison.Ordinal);
        }
    }
}