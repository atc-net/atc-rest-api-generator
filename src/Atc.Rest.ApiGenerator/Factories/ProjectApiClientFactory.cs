namespace Atc.Rest.ApiGenerator.Factories;

public static class ProjectApiClientFactory
{
    public static string[] CreateUsingListForEndpointInterface(
        ApiProjectOptions apiProjectOptions,
        bool includeRestResults,
        bool hasList,
        bool hasSharedModel)
    {
        ArgumentNullException.ThrowIfNull(apiProjectOptions);

        var list = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.Threading",
            "System.Threading.Tasks",
        };

        if (hasList)
        {
            list.Add("System.Collections.Generic");
        }

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

        if (!list.Contains(s, StringComparer.Ordinal))
        {
            list.Add(s);
        }

        return list.ToArray();
    }

    public static string[] CreateUsingListForEndpoint(
        ApiProjectOptions apiProjectOptions,
        bool includeRestResults,
        bool hasParameter,
        bool hasList,
        bool hasSharedModel)
    {
        ArgumentNullException.ThrowIfNull(apiProjectOptions);

        var list = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.Net",
            "System.Net.Http",
            "System.Threading",
            "System.Threading.Tasks",
            "Atc.Rest.Client.Builder",
            "Microsoft.AspNetCore.Mvc",
        };

        if (hasParameter)
        {
            list.Add("System");
        }

        if (hasList)
        {
            list.Add("System.Collections.Generic");
        }

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

        if (!list.Contains(s, StringComparer.Ordinal))
        {
            list.Add(s);
        }

        return list.ToArray();
    }

    public static string[] CreateUsingListForEndpointResultInterface(
        ApiProjectOptions apiProjectOptions,
        bool includeRestResults,
        bool hasList,
        bool hasSharedModel)
    {
        ArgumentNullException.ThrowIfNull(apiProjectOptions);

        var list = new List<string>
        {
            "System.CodeDom.Compiler",
            "Atc.Rest.Client",
            "Microsoft.AspNetCore.Mvc",
        };

        if (hasList)
        {
            list.Add("System.Collections.Generic");
        }

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

        if (!list.Contains(s, StringComparer.Ordinal))
        {
            list.Add(s);
        }

        return list.ToArray();
    }

    public static string[] CreateUsingListForEndpointResult(
        ApiProjectOptions apiProjectOptions,
        bool includeRestResults,
        bool hasList,
        bool hasSharedModel)
    {
        ArgumentNullException.ThrowIfNull(apiProjectOptions);

        var list = new List<string>
        {
            "System",
            "System.Net",
            "System.CodeDom.Compiler",
            "Atc.Rest.Client",
            "Microsoft.AspNetCore.Mvc",
        };

        if (hasList)
        {
            list.Add("System.Collections.Generic");
        }

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

        if (!list.Contains(s, StringComparer.Ordinal))
        {
            list.Add(s);
        }

        return list.ToArray();
    }
}