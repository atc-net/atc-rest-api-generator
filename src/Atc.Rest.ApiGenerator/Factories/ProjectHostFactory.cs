// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.Factories;

public static class ProjectHostFactory
{
    public static string[] CreateUsingListForProgram()
    {
        var list = new List<string>
        {
            "Microsoft.AspNetCore.Hosting",
            "Microsoft.Extensions.Hosting",
        };

        return list.ToArray();
    }

    public static string[] CreateUsingListForStartup(
        string projectName,
        bool useExtended)
    {
        ArgumentNullException.ThrowIfNull(projectName);

        var name = projectName.Replace(".Api", string.Empty, StringComparison.Ordinal);

        var list = new List<string>
        {
            "System.Reflection",
            useExtended
                ? "Atc.Rest.Extended.Options"
                : "Atc.Rest.Options",
            "Microsoft.AspNetCore.Builder",
            "Microsoft.AspNetCore.Hosting",
            "Microsoft.Extensions.Configuration",
            "Microsoft.Extensions.DependencyInjection",
            $"{name}.Api.Generated",
            $"{name}.Domain",
        };

        return list.ToArray();
    }

    public static string[] CreateUsingListForWebApiControllerBaseTest()
    {
        var list = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.Collections.Generic",
            "System.IO",
            "System.Net.Http",
            "System.Text",
            "System.Text.Json",
            "System.Text.Json.Serialization",
            "Microsoft.AspNetCore.Http",
            "Microsoft.Extensions.Configuration",
            "Xunit",
        };

        return list.ToArray();
    }

    public static string[] CreateUsingListForWebApiStartupFactory(
        string projectName)
    {
        ArgumentNullException.ThrowIfNull(projectName);

        var name = projectName.Replace(".Api", string.Empty, StringComparison.Ordinal);

        var list = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.Reflection",
            "Atc.Rest.Options",
            "Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Mvc.Testing",
            "Microsoft.AspNetCore.TestHost",
            "Microsoft.Extensions.Configuration",
            "Microsoft.Extensions.DependencyInjection",
            $"{name}.Api.Generated",
        };

        return list.ToArray();
    }
}