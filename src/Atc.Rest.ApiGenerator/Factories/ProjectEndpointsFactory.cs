using System;
using System.Collections.Generic;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.Factories
{
    internal static class ProjectEndpointsFactory
    {
        public static string[] CreateUsingList(
            ApiProjectOptions apiProjectOptions,
            string focusOnSegmentName,
            List<OpenApiOperation> apiOperations,
            bool includeRestResults,
            bool hasSharedModel,
            bool forClient = false)
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

            if (hasSharedModel)
            {
                if (forClient)
                {
                    list.Add(string.IsNullOrEmpty(apiProjectOptions.ClientFolderName)
                        ? $"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}"
                        : $"{apiProjectOptions.ProjectName}.{apiProjectOptions.ClientFolderName}.{NameConstants.Contracts}");
                }
                else
                {
                    list.Add($"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}");
                }
            }

            if (forClient)
            {
                list.Add(string.IsNullOrEmpty(apiProjectOptions.ClientFolderName)
                    ? $"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}.{focusOnSegmentName.EnsureFirstCharacterToUpper()}"
                    : $"{apiProjectOptions.ProjectName}.{apiProjectOptions.ClientFolderName}.{NameConstants.Contracts}.{focusOnSegmentName.EnsureFirstCharacterToUpper()}");
            }
            else
            {
                list.Add($"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}.{focusOnSegmentName.EnsureFirstCharacterToUpper()}");
            }

            if (forClient)
            {
                list.Add("System.Net");
                list.Add("System.Net.Http");
                list.Add("Atc.Rest.Client");
            }

            list.Add("Microsoft.AspNetCore.Http");
            list.Add("Microsoft.AspNetCore.Mvc");

            if (apiOperations.HasDataTypeFromSystemCollectionGenericNamespace())
            {
                list.Add("System.Collections.Generic");
            }
            else if (forClient)
            {
                list.Add("System.Collections.Generic");
            }

            if (apiProjectOptions.ApiOptions.Generator.UseAuthorization)
            {
                list.Add("Microsoft.AspNetCore.Authorization");
            }

            return list.ToArray();
        }
    }
}