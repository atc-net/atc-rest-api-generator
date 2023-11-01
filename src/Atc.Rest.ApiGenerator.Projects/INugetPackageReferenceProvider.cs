namespace Atc.Rest.ApiGenerator.Projects;

public interface INugetPackageReferenceProvider
{
    Task<Version> GetAtcApiGeneratorVersion();

    Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForHostProjectForMvc(
            bool useRestExtended);

    Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForHostProjectForMinimalApi();

    Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForApiProjectForMvc();

    Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForApiProjectForMinimalApi();

    Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesBaseLineForApiClientProject();

    IList<(string PackageId, string PackageVersion, string? SubElements)> GetPackageReferencesBaseLineForTestProject(
            bool useMvc);
}