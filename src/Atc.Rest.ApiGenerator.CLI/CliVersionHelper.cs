namespace Atc.Rest.ApiGenerator.CLI;

public static class CliVersionHelper
{
    public static Version? GetLatestVersion()
        => AtcApiNugetClientHelper.GetLatestVersionForPackageId("atc-rest-api-generator");

    public static bool IsLatestVersion()
    {
        var currentVersion = CliHelper.GetCurrentVersion();
        if (currentVersion == new Version(1, 0, 0, 0))
        {
            return true;
        }

        var latestVersion = GetLatestVersion();
        return latestVersion is null ||
               !latestVersion.GreaterThan(currentVersion);
    }
}