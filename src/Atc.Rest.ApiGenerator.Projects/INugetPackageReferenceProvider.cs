namespace Atc.Rest.ApiGenerator.Projects;

public interface INugetPackageReferenceProvider
{
    Task<Version> GetAtcApiGeneratorVersion();

    Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForHostProject(
            bool useRestExtended);

    Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForApiProject();

    Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForApiClientProject();

    IList<(string PackageId, string PackageVersion, string? SubElements)> GetPackageReferencesBaseLineForTestProject(
            bool useMvc);
}