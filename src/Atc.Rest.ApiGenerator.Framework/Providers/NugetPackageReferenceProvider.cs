namespace Atc.Rest.ApiGenerator.Framework.Providers;

[SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "OK - for now")]
public class NugetPackageReferenceProvider(
    IAtcApiNugetClient atcApiNugetClient)
    : INugetPackageReferenceProvider
{
    private Dictionary<string, string> PackageDefaultVersions { get; } = new(StringComparer.Ordinal)
    {
        { "Asp.Versioning.Http", "8.1.0" },
        { "Atc", "2.0.525" },
        { "Atc.Azure.Options", "3.0.34" },
        { "Atc.Rest", "2.0.525" },
        { "Atc.Rest.Client", "1.0.84" },
        { "Atc.Rest.Extended", "2.0.525" },
        { "Atc.Rest.FluentAssertions", "2.0.525" },
        { "Atc.Rest.MinimalApi", "1.0.87" },
        { "Atc.XUnit", "2.0.525" },
        { "AutoFixture", "4.18.1" },
        { "AutoFixture.AutoNSubstitute", "4.18.1" },
        { "AutoFixture.Xunit2", "4.18.1" },
        { "FluentAssertions", "7.2.0" },
        { "FluentValidation.AspNetCore", "11.3.0" },
        { "Microsoft.ApplicationInsights.AspNetCore", "2.22.0" },
        { "Microsoft.AspNetCore.Authentication.JwtBearer", "8.0.4" },
        { "Microsoft.AspNetCore.OpenApi", "8.0.6" },
        { "Microsoft.AspNetCore.Mvc.Testing", "8.0.6" },
        { "Microsoft.NETCore.Platforms", "7.0.4" },
        { "Microsoft.NET.Test.Sdk", "17.13.0" },
        { "NSubstitute", "5.3.0" },
        { "Swashbuckle.AspNetCore", "7.3.1" },
        { "xunit", "2.9.3" },
        { "xunit.runner.visualstudio", "3.0.2" },
    };

    public async Task<Version> GetAtcApiGeneratorVersion()
    {
        var version = await atcApiNugetClient.RetrieveLatestVersionForPackageId(
            "Atc.Rest.ApiGenerator",
            CancellationToken.None) ?? new Version(2, 0, 360, 0);

        var assemblyVersion = CliHelper.GetCurrentVersion();

        return assemblyVersion.GreaterThan(version)
            ? assemblyVersion
            : new Version(2, 0, 360, 0);
    }

    public async Task<IList<(string PackageId, string PackageVersion)>> GetPackageReferencesForHostProjectForMvc(
        bool useRestExtended)
    {
        var atcVersion = await GetAtcVersionAsString3();

        if (useRestExtended)
        {
            var packageReferences = new List<(string, string)>
            {
                ("Asp.Versioning.Http", PackageDefaultVersions["Asp.Versioning.Http"]),
                ("Atc", atcVersion),
                ("Atc.Rest.Extended", atcVersion),
                ("Microsoft.NETCore.Platforms", PackageDefaultVersions["Microsoft.NETCore.Platforms"]),
                ("Swashbuckle.AspNetCore", PackageDefaultVersions["Swashbuckle.AspNetCore"]),
            };

            return packageReferences;
        }
        else
        {
            var packageReferences = new List<(string, string)>
            {
                new("Atc", atcVersion),
                new("Atc.Rest", atcVersion),
            };

            return packageReferences;
        }
    }

    public async Task<IList<(string PackageId, string PackageVersion)>> GetPackageReferencesForHostProjectForMinimalApi()
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string)>
        {
            new("Asp.Versioning.Http", PackageDefaultVersions["Asp.Versioning.Http"]),
            new("Atc", atcVersion),
            new("Atc.Rest.Extended", atcVersion),
            new("Atc.Rest.MinimalApi", PackageDefaultVersions["Atc.Rest.MinimalApi"]),
            new("Microsoft.NETCore.Platforms", PackageDefaultVersions["Microsoft.NETCore.Platforms"]),
            new("Swashbuckle.AspNetCore", PackageDefaultVersions["Swashbuckle.AspNetCore"]),
        };

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion)>> GetPackageReferencesForApiProjectForMvc()
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string)>
        {
            new("Asp.Versioning.Http", PackageDefaultVersions["Asp.Versioning.Http"]),
            new("Atc", atcVersion),
            new("Atc.Rest", atcVersion),
            new("FluentValidation.AspNetCore", PackageDefaultVersions["FluentValidation.AspNetCore"]),
            new("Microsoft.AspNetCore.OpenApi", PackageDefaultVersions["Microsoft.AspNetCore.OpenApi"]),
            new("Microsoft.NETCore.Platforms", PackageDefaultVersions["Microsoft.NETCore.Platforms"]),
        };

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion)>> GetPackageReferencesForApiProjectForMinimalApi()
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string)>
        {
            new("Asp.Versioning.Http", PackageDefaultVersions["Asp.Versioning.Http"]),
            new("Atc", atcVersion),
            new("Atc.Rest", atcVersion),
            new("Atc.Rest.MinimalApi", PackageDefaultVersions["Atc.Rest.MinimalApi"]),
            new("FluentValidation.AspNetCore", PackageDefaultVersions["FluentValidation.AspNetCore"]),
            new("Microsoft.AspNetCore.OpenApi", PackageDefaultVersions["Microsoft.AspNetCore.OpenApi"]),
            new("Microsoft.NETCore.Platforms", PackageDefaultVersions["Microsoft.NETCore.Platforms"]),
        };

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion)>?> GetPackageReferencesForDomainProjectForMvc()
    {
        var packageReferences = new List<(string, string)>();

        await Task.CompletedTask;

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion)>?> GetPackageReferencesForDomainProjectForMinimalApi()
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string)>
        {
            new("Atc.Azure.Options", PackageDefaultVersions["Atc.Azure.Options"]),
            new("Atc.Rest", atcVersion),
        };

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesForApiClientProject()
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string, string?)>
        {
            new("Atc", atcVersion, null),
            new("Atc.Rest", atcVersion, null),
            new("Atc.Rest.Client", PackageDefaultVersions["Atc.Rest.Client"], null),
        };

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesForTestHostProjectForMvc()
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string, string?)>
        {
            new("Atc.Rest.FluentAssertions", atcVersion, null),
            new("Atc.XUnit", atcVersion, null),
            new("Microsoft.AspNetCore.Mvc.Testing", PackageDefaultVersions["Microsoft.AspNetCore.Mvc.Testing"], null),
            new("Microsoft.NET.Test.Sdk", PackageDefaultVersions["Microsoft.NET.Test.Sdk"], null),
            new("xunit", PackageDefaultVersions["xunit"], null),
            new("xunit.runner.visualstudio", PackageDefaultVersions["xunit.runner.visualstudio"], "<PrivateAssets>all</PrivateAssets>\n<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>"),
        };

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesForTestHostProjectForMinimalApi()
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string, string?)>
        {
            new("Atc.Rest.FluentAssertions", atcVersion, null),
            new("Atc.XUnit", atcVersion, null),
            new("AutoFixture", PackageDefaultVersions["AutoFixture"], null),
            new("AutoFixture.AutoNSubstitute", PackageDefaultVersions["AutoFixture.AutoNSubstitute"], null),
            new("AutoFixture.Xunit2", PackageDefaultVersions["AutoFixture.Xunit2"], null),
            new("FluentAssertions", PackageDefaultVersions["FluentAssertions"], null),
            new("Microsoft.NET.Test.Sdk", PackageDefaultVersions["Microsoft.NET.Test.Sdk"], null),
            new("xunit", PackageDefaultVersions["xunit"], null),
            new("xunit.runner.visualstudio", PackageDefaultVersions["xunit.runner.visualstudio"], "<PrivateAssets>all</PrivateAssets>\n<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>"),
        };

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesForTestDomainProjectForMvc()
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string, string?)>
        {
            new("Atc.XUnit", atcVersion, null),
            new("AutoFixture", PackageDefaultVersions["AutoFixture"], null),
            new("AutoFixture.AutoNSubstitute", PackageDefaultVersions["AutoFixture.AutoNSubstitute"], null),
            new("AutoFixture.Xunit2", PackageDefaultVersions["AutoFixture.Xunit2"], null),
            new("FluentAssertions", PackageDefaultVersions["FluentAssertions"], null),
            new("Microsoft.AspNetCore.Mvc.Testing", PackageDefaultVersions["Microsoft.AspNetCore.Mvc.Testing"], null),
            new("Microsoft.NET.Test.Sdk", PackageDefaultVersions["Microsoft.NET.Test.Sdk"], null),
            new("NSubstitute", PackageDefaultVersions["NSubstitute"], null),
            new("xunit", PackageDefaultVersions["xunit"], null),
            new("xunit.runner.visualstudio", PackageDefaultVersions["xunit.runner.visualstudio"], "<PrivateAssets>all</PrivateAssets>\n<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>"),
        };

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesForTestDomainProjectForMinimalApi()
    {
        var packageReferences = new List<(string, string, string?)>();

        await Task.CompletedTask;

        return packageReferences;
    }

    private async Task<Version> GetAtcVersion()
    {
        var version = await atcApiNugetClient.RetrieveLatestVersionForPackageId(
            "Atc",
            CancellationToken.None);
        return version ?? new Version(PackageDefaultVersions["Atc"]);
    }

    private async Task<string> GetAtcVersionAsString3()
    {
        var atcVersion = await GetAtcVersion();
        return $"{atcVersion.Major}.{atcVersion.Minor}.{atcVersion.Build}";
    }
}