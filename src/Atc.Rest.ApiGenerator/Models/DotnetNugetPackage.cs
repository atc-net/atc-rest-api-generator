namespace Atc.Rest.ApiGenerator.Models;

public class DotnetNugetPackage
{
    public DotnetNugetPackage(string packageId, Version currentVersion)
    {
        PackageId = packageId;
        Version = currentVersion;
        NewestVersion = currentVersion;
    }

    public DotnetNugetPackage(string packageId, Version currentVersion, Version newestVersion)
    {
        PackageId = packageId;
        Version = currentVersion;
        NewestVersion = newestVersion;
    }

    public string PackageId { get; }

    public Version Version { get; }

    public Version NewestVersion { get; set; }

    public bool IsNewest => Version >= NewestVersion;

    public override string ToString()
        => $"{nameof(PackageId)}: {PackageId}, {nameof(Version)}: {Version}, {nameof(NewestVersion)}: {NewestVersion}, {nameof(IsNewest)}: {IsNewest}";
}