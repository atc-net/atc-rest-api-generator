using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.Factories
{
    public static class ProjectApiFactory
    {
        public static string[] CreateUsingListForEndpoint(
            ApiProjectOptions apiProjectOptions,
            List<OpenApiOperation> apiOperations,
            bool hasSharedModel,
            bool includeRestResults,
            string focusOnSegmentName)
        {
            if (apiOperations == null)
            {
                throw new ArgumentNullException(nameof(apiOperations));
            }

            var list = new List<string>
            {
                "System",
                "System.CodeDom.Compiler",
                "System.Threading",
                "System.Threading.Tasks",
            };

            if (includeRestResults)
            {
                list.Add("Atc.Rest.Results");
            }

            list.Add("Microsoft.AspNetCore.Http");
            list.Add("Microsoft.AspNetCore.Mvc");

            if (apiOperations.HasDataTypeFromSystemCollectionGenericNamespace())
            {
                list.Add("System.Collections.Generic");
            }

            if (apiProjectOptions.ApiOptions.Generator.UseAuthorization)
            {
                list.Add("Microsoft.AspNetCore.Authorization");
            }

            if (hasSharedModel)
            {
                list.Add($"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}");
            }

            list.Add($"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}.{focusOnSegmentName.EnsureFirstCharacterToUpper()}");

            return list.ToArray();
        }

        public static string[] CreateUsingListForContractInterface()
        {
            return new[]
            {
                "System.CodeDom.Compiler",
                "System.Threading",
                "System.Threading.Tasks",
            };
        }

        public static string[] CreateUsingListForContractModel(OpenApiSchema? apiSchema)
        {
            if (apiSchema == null)
            {
                throw new ArgumentNullException(nameof(apiSchema));
            }

            var list = new List<string>();
            var schemasToCheck = apiSchema.Properties
                .Select(x => x.Value)
                .ToList();

            if (schemasToCheck.HasFormatTypeFromSystemNamespace())
            {
                list.Add("System");
            }

            list.Add("System.CodeDom.Compiler");

            if (apiSchema.IsTypeArray() ||
                schemasToCheck.HasDataTypeFromSystemCollectionGenericNamespace())
            {
                list.Add("System.Collections.Generic");
            }

            if (apiSchema.Required.Any() ||
                schemasToCheck.HasFormatTypeFromDataAnnotationsNamespace())
            {
                list.Add("System.ComponentModel.DataAnnotations");
            }

            if (schemasToCheck.HasFormatTypeFromAspNetCoreHttpNamespace())
            {
                list.Add("Microsoft.AspNetCore.Http");
            }

            return list.ToArray();
        }

        public static string[] CreateUsingListForContractParameter(
            IList<OpenApiParameter>? globalParameters,
            IList<OpenApiParameter>? parameters,
            OpenApiRequestBody? requestBody,
            bool forClient)
        {
            var list = new List<string>();

            if (globalParameters != null)
            {
                if (globalParameters.HasFormatTypeFromSystemNamespace() ||
                    globalParameters.Any(x => x.Schema.IsTypeArray()))
                {
                    list.Add("System");
                }

                if (ShouldUseDataAnnotationsNamespace(globalParameters))
                {
                    list.Add("System.ComponentModel.DataAnnotations");
                }
            }

            if (parameters != null)
            {
                if (list.All(x => x != "System") &&
                    (parameters.HasFormatTypeFromSystemNamespace() ||
                    parameters.Any(x => x.Schema.IsTypeArray())))
                {
                    list.Add("System");
                }

                if (list.All(x => x != "System.Collections.Generic") &&
                    parameters.Any(x => x.Schema.IsTypeArray() || x.Schema.HasDataTypeFromSystemCollectionGenericNamespace()))
                {
                    list.Add("System.Collections.Generic");
                }

                list.Add("System.CodeDom.Compiler");

                if (list.All(x => x != "System.ComponentModel.DataAnnotations") &&
                    ShouldUseDataAnnotationsNamespace(parameters))
                {
                    list.Add("System.ComponentModel.DataAnnotations");
                }

                if (!forClient)
                {
                    list.Add("Microsoft.AspNetCore.Mvc");
                }
            }

            var contentSchema = requestBody?.Content?.GetSchemaByFirstMediaType();
            if (contentSchema != null)
            {
                if (list.All(x => x != "System") &&
                    contentSchema.HasFormatTypeFromSystemNamespace())
                {
                    list.Add("System");
                }

                if (list.All(x => x != "System.Collections.Generic") &&
                    (contentSchema.IsTypeArray() || contentSchema.HasDataTypeFromSystemCollectionGenericNamespace()))
                {
                    list.Add("System.Collections.Generic");
                }

                if (list.All(x => x != "System.ComponentModel.DataAnnotations"))
                {
                    list.Add("System.ComponentModel.DataAnnotations");
                }

                if (contentSchema.IsFormatTypeBinary() || contentSchema.HasItemsWithFormatTypeBinary())
                {
                    list.Add("Microsoft.AspNetCore.Http");
                }
            }

            return list.ToArray();
        }

        public static string[] CreateUsingListForContractResult(
            OpenApiResponses responses,
            bool useProblemDetailsAsDefaultResponseBody,
            bool hasCreateContentResult)
        {
            if (responses == null)
            {
                throw new ArgumentNullException(nameof(responses));
            }

            var list = new List<string>
            {
                "System.CodeDom.Compiler",
                "Microsoft.AspNetCore.Mvc",
            };

            if (responses.HasSchemaTypeArray())
            {
                list.Add("System.Linq");
                list.Add("System.Collections.Generic");
            }

            if (hasCreateContentResult)
            {
                if (responses.GetHttpStatusCodes().Any())
                {
                    list.Add("System.Net");
                }
            }
            else
            {
                if (responses.GetHttpStatusCodes().Any(x => x != HttpStatusCode.OK))
                {
                    list.Add("System.Net");
                }
            }

            if (useProblemDetailsAsDefaultResponseBody)
            {
                list.Add("Atc.Rest.Results");
            }
            else
            {
                list.Add("Atc.Rest.Results");

                if (responses.HasSchemaHttpStatusCodeUsingAspNetCoreHttp())
                {
                    list.Add("Microsoft.AspNetCore.Http");
                }
            }

            return list.ToArray();
        }

        private static bool ShouldUseDataAnnotationsNamespace(IList<OpenApiParameter> parameters)
            => parameters.Any(x => x.Required) || parameters.HasFormatTypeFromDataAnnotationsNamespace();
    }
}