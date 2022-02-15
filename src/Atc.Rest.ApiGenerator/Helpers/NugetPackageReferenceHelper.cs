// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.Helpers;

public static class NugetPackageReferenceHelper
{
    public static List<Tuple<string, string, string?>> CreateForHostProject(bool useRestExtended)
    {
        var atcVersion = GenerateHelper.GetAtcVersionAsString3();

        var packageReference = new List<Tuple<string, string, string?>>
        {
            new ("Atc", atcVersion, null),
            new ("Atc.Rest", atcVersion, null),
        };

        if (useRestExtended)
        {
            packageReference.Add(new Tuple<string, string, string?>("Atc.Rest.Extended", atcVersion, null));
            packageReference.Add(new Tuple<string, string, string?>("FluentValidation.AspNetCore", "10.3.6", null));
            packageReference.Add(new Tuple<string, string, string?>("Microsoft.ApplicationInsights.AspNetCore", "2.20.0", null));
            packageReference.Add(new Tuple<string, string, string?>("Microsoft.AspNetCore.Authentication.JwtBearer", "6.0.2", null));
            packageReference.Add(new Tuple<string, string, string?>("Microsoft.AspNetCore.Mvc.Versioning", "5.0.0", null));
            packageReference.Add(new Tuple<string, string, string?>("Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer", "5.0.0", null));
            packageReference.Add(new Tuple<string, string, string?>("Swashbuckle.AspNetCore", "6.2.3", null));
        }

        return packageReference;
    }

    public static List<Tuple<string, string, string?>> CreateForApiProject()
    {
        var atcVersion = GenerateHelper.GetAtcVersionAsString3();

        var packageReference = new List<Tuple<string, string, string?>>
        {
            new ("Atc", atcVersion, null),
            new ("Atc.Rest", atcVersion, null),
        };

        return packageReference;
    }

    public static List<Tuple<string, string, string?>> CreateForClientApiProject()
    {
        var atcVersion = GenerateHelper.GetAtcVersionAsString3();

        var packageReference = new List<Tuple<string, string, string?>>
        {
            new ("Atc", atcVersion, null),
            new ("Atc.Rest.Client", "1.0.31", null),
        };

        return packageReference;
    }

    public static List<Tuple<string, string, string?>> CreateForTestProject(bool useMvc)
    {
        var packageReference = new List<Tuple<string, string, string?>>
        {
            new ("AutoFixture", "4.17.0", null),
            new ("AutoFixture.AutoNSubstitute", "4.17.0", null),
            new ("AutoFixture.Xunit2", "4.17.0", null),
            new ("FluentAssertions", "6.5.1", null),
        };

        if (useMvc)
        {
            packageReference.Add(new Tuple<string, string, string?>("Microsoft.AspNetCore.Mvc.Testing", "3.1.18", null));
        }

        packageReference.AddRange(new List<Tuple<string, string, string?>>
        {
            new ("Microsoft.NET.Test.Sdk", "17.0.0", null),
            new ("NSubstitute", "4.3.0", null),
            new ("xunit", "2.4.1", null),
            new ("xunit.runner.visualstudio", "2.4.3", "<PrivateAssets>all</PrivateAssets>\n<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>"),
        });

        return packageReference;
    }
}