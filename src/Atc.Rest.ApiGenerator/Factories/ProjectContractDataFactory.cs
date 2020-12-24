using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.Factories
{
    internal static class ProjectContractDataFactory
    {
        public static string[] CreateUsingList(OpenApiSchema? apiSchema)
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

            if (apiSchema.Type == OpenApiDataTypeConstants.Array ||
                schemasToCheck.HasDataTypeFromSystemCollectionGenericNamespace())
            {
                list.Add("System.Collections.Generic");
            }

            if (apiSchema.Required.Any() ||
                schemasToCheck.HasFormatTypeFromDataAnnotationsNamespace())
            {
                list.Add("System.ComponentModel.DataAnnotations");
            }

            return list.ToArray();
        }
    }
}