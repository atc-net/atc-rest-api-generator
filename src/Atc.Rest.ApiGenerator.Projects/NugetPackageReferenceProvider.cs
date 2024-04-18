namespace Atc.Rest.ApiGenerator.Projects;

public class NugetPackageReferenceProvider : INugetPackageReferenceProvider
{
    private readonly IAtcApiNugetClient atcApiNugetClient;

    public NugetPackageReferenceProvider(
        IAtcApiNugetClient atcApiNugetClient)
    {
        this.atcApiNugetClient = atcApiNugetClient;
    }

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

    public async Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForHostProjectForMvc(
        bool useRestExtended)
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string, string?)>
        {
            new("Atc", atcVersion, null),
            new("Atc.Rest", atcVersion, null),
        };

        if (!useRestExtended)
        {
            return packageReferences;
        }

        packageReferences.Add(("Asp.Versioning.Http", "8.1.0", null));
        packageReferences.Add(("Atc.Rest.Extended", atcVersion, null));
        packageReferences.Add(("FluentValidation.AspNetCore", "11.3.0", null));
        packageReferences.Add(("Microsoft.ApplicationInsights.AspNetCore", "2.22.0", null));
        packageReferences.Add(("Microsoft.AspNetCore.Authentication.JwtBearer", "8.0.3", null));
        packageReferences.Add(("Swashbuckle.AspNetCore", "6.5.0", null));

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForHostProjectForMinimalApi()
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string, string?)>
        {
            new("Asp.Versioning.Http", "8.1.0", null),
            new("Atc", atcVersion, null),
            new("Atc.Rest.Extended", atcVersion, null),
            new("Microsoft.NETCore.Platforms", "7.0.4", null),
            new("Swashbuckle.AspNetCore", "6.5.0", null),
        };

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForApiProjectForMvc()
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string, string?)>
        {
            new("Atc", atcVersion, null),
            new("Atc.Rest", atcVersion, null),
        };

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForApiProjectForMinimalApi()
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string, string?)>
        {
            new("Asp.Versioning.Http", "8.1.0", null),
            new("Atc", atcVersion, null),
            new("Atc.Rest", atcVersion, null),
            new("Atc.Rest.MinimalApi", "1.0.60", null),
            new("FluentValidation.AspNetCore", "11.3.0", null),
            new("Microsoft.AspNetCore.OpenApi", "8.0.3", null),
            new("Microsoft.NETCore.Platforms", "7.0.4", null),
        };

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForApiClientProject()
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string, string?)>
        {
            new("Atc", atcVersion, null),
            new("Atc.Rest", atcVersion, null),
            new("Atc.Rest.Client", "1.0.36", null),
        };

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion, string? SubElements)>?> GetPackageReferencesBaseLineForDomainProjectForMinimalApi()
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string, string?)>
        {
            new("Atc.Azure.Options", "3.0.28", null),
            new("Atc.Rest", atcVersion, null),
        };

        return packageReferences;
    }

    public async Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForTestProject(
        bool useMvc)
    {
        var atcVersion = await GetAtcVersionAsString3();

        var packageReferences = new List<(string, string, string?)>
        {
            new("Atc.XUnit", atcVersion, null),
            new("AutoFixture", "4.18.1", null),
            new("AutoFixture.AutoNSubstitute", "4.18.1", null),
            new("AutoFixture.Xunit2", "4.18.1", null),
            new("FluentAssertions", "6.12.0", null),
        };

        if (useMvc)
        {
            packageReferences.Add(("Microsoft.AspNetCore.Mvc.Testing", "8.0.3", null));
        }

        packageReferences.AddRange(new List<(string, string, string?)>
        {
            new("Microsoft.NET.Test.Sdk", "17.9.0", null),
            new("NSubstitute", "5.1.0", null),
            new("xunit", "2.7.0", null),
            new("xunit.runner.visualstudio", "2.5.7", "<PrivateAssets>all</PrivateAssets>\n<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>"),
        });

        return packageReferences;
    }

    private async Task<Version> GetAtcVersion()
    {
        var version = await atcApiNugetClient.RetrieveLatestVersionForPackageId(
            "Atc",
            CancellationToken.None);
        return version ?? new Version(2, 0, 280, 0);
    }

    private async Task<string> GetAtcVersionAsString3()
    {
        var atcVersion = await GetAtcVersion();
        return $"{atcVersion.Major}.{atcVersion.Minor}.{atcVersion.Build}";
    }
}