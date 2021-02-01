using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Atc.Rest.ApiGenerator.Models;

namespace Atc.Rest.ApiGenerator.Factories
{
    [SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "OK for now, need to be optimized.")]
    public static class ProjectApiClientFactory
    {
        public static string[] CreateUsingListForEndpointInterface(
            ApiProjectOptions apiProjectOptions,
            bool includeRestResults,
            bool hasSharedModel)
        {
            var list = new List<string>
            {
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
                list.Add(string.IsNullOrEmpty(apiProjectOptions.ClientFolderName)
                    ? $"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}"
                    : $"{apiProjectOptions.ProjectName}.{apiProjectOptions.ClientFolderName}.{NameConstants.Contracts}");
            }

            var s = string.IsNullOrEmpty(apiProjectOptions.ClientFolderName)
                ? $"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}"
                : $"{apiProjectOptions.ProjectName}.{apiProjectOptions.ClientFolderName}.{NameConstants.Contracts}";

            if (!list.Contains(s))
            {
                list.Add(s);
            }

            list.Add("Atc.Rest.Client");

            list.Add("System.Collections.Generic");

            return list.ToArray();
        }

        public static string[] CreateUsingListForEndpoint(
            ApiProjectOptions apiProjectOptions,
            bool includeRestResults,
            bool hasSharedModel)
        {
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
                list.Add(string.IsNullOrEmpty(apiProjectOptions.ClientFolderName)
                    ? $"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}"
                    : $"{apiProjectOptions.ProjectName}.{apiProjectOptions.ClientFolderName}.{NameConstants.Contracts}");
            }

            var s = string.IsNullOrEmpty(apiProjectOptions.ClientFolderName)
                ? $"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}"
                : $"{apiProjectOptions.ProjectName}.{apiProjectOptions.ClientFolderName}.{NameConstants.Contracts}";

            if (!list.Contains(s))
            {
                list.Add(s);
            }

            list.Add("System.Net");
            list.Add("System.Net.Http");
            list.Add("Atc.Rest.Client");
            list.Add("Atc.Rest.Client.Builder");

            list.Add("Microsoft.AspNetCore.Mvc");

            list.Add("System.Collections.Generic"); // TODO: Remove when switching from EndpointResult -> XXXEndpointResult

            return list.ToArray();
        }

        public static string[] CreateUsingListForEndpointResultInterface(
            ApiProjectOptions apiProjectOptions,
            bool includeRestResults,
            bool hasSharedModel)
        {
            var list = new List<string>
            {
                "System.CodeDom.Compiler",
            };

            if (includeRestResults)
            {
                list.Add("Atc.Rest.Results");
            }

            if (hasSharedModel)
            {
                list.Add(string.IsNullOrEmpty(apiProjectOptions.ClientFolderName)
                    ? $"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}"
                    : $"{apiProjectOptions.ProjectName}.{apiProjectOptions.ClientFolderName}.{NameConstants.Contracts}");
            }

            var s = string.IsNullOrEmpty(apiProjectOptions.ClientFolderName)
                ? $"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}"
                : $"{apiProjectOptions.ProjectName}.{apiProjectOptions.ClientFolderName}.{NameConstants.Contracts}";

            if (!list.Contains(s))
            {
                list.Add(s);
            }

            list.Add("System.Collections.Generic");

            return list.ToArray();
        }

        public static string[] CreateUsingListForEndpointResult(
            ApiProjectOptions apiProjectOptions,
            bool includeRestResults,
            bool hasSharedModel)
        {
            var list = new List<string>
            {
                "System",
                "System.CodeDom.Compiler",
            };

            if (includeRestResults)
            {
                list.Add("Atc.Rest.Results");
            }

            if (hasSharedModel)
            {
                list.Add(string.IsNullOrEmpty(apiProjectOptions.ClientFolderName)
                    ? $"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}"
                    : $"{apiProjectOptions.ProjectName}.{apiProjectOptions.ClientFolderName}.{NameConstants.Contracts}");
            }

            var s = string.IsNullOrEmpty(apiProjectOptions.ClientFolderName)
                ? $"{apiProjectOptions.ProjectName}.{NameConstants.Contracts}"
                : $"{apiProjectOptions.ProjectName}.{apiProjectOptions.ClientFolderName}.{NameConstants.Contracts}";

            if (!list.Contains(s))
            {
                list.Add(s);
            }

            list.Add("System.Net");
            list.Add("Atc.Rest.Client");
            list.Add("Microsoft.AspNetCore.Mvc");

            list.Add("System.Collections.Generic");

            return list.ToArray();
        }
    }
}
