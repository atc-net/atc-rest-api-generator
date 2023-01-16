namespace Atc.Rest.ApiGenerator.Nuget.Clients;

public interface IAtcApiNugetClient
{
    Task<Version?> RetrieveLatestVersionForPackageId(
        string packageId,
        CancellationToken cancellationToken = default);
}