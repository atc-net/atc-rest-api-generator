namespace Atc.Rest.ApiGenerator.CLI;

public static class CliVersionHelper
{
    public static Version GetCurrentVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
        if (fileVersion is null)
        {
            return new Version(1, 0, 0, 0);
        }

        return Version.TryParse(fileVersion, out var version)
            ? version
            : new Version(1, 0, 0, 0);
    }

    public static Version? GetLatestVersion()
        => AtcApiNugetClientHelper.GetLatestVersionForPackageId("atc-rest-api-generator");

    public static bool IsLatestVersion()
    {
        var currentVersion = GetCurrentVersion();
        if (currentVersion == new Version(1, 0, 0, 0))
        {
            return true;
        }

        var latestVersion = GetLatestVersion();
        return latestVersion is null || !latestVersion.GreaterThan(currentVersion);
    }
}