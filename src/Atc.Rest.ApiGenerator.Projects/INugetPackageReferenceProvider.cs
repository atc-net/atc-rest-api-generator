namespace Atc.Rest.ApiGenerator.Projects;

public interface INugetPackageReferenceProvider
{
    Task<Version> GetAtcApiGeneratorVersion();

    Task<IList<(string PackageId, string PackageVersion)>> GetPackageReferencesForHostProjectForMvc(
            bool useRestExtended);

    Task<IList<(string PackageId, string PackageVersion)>> GetPackageReferencesForHostProjectForMinimalApi();

    Task<IList<(string PackageId, string PackageVersion)>> GetPackageReferencesForApiProjectForMvc();

    Task<IList<(string PackageId, string PackageVersion)>> GetPackageReferencesForApiProjectForMinimalApi();

    Task<IList<(string PackageId, string PackageVersion)>?> GetPackageReferencesForDomainProjectForMvc();

    Task<IList<(string PackageId, string PackageVersion)>?> GetPackageReferencesForDomainProjectForMinimalApi();

    Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesForApiClientProject();

    Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesForTestHostProjectForMvc();

    Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesForTestHostProjectForMinimalApi();

    Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesForTestDomainProjectForMvc();

    Task<IList<(string PackageId, string PackageVersion, string? SubElements)>> GetPackageReferencesForTestDomainProjectForMinimalApi();
}