namespace Atc.Rest.ApiGenerator.Helpers;

public static class HttpClientHelper
{
    private static readonly ConcurrentDictionary<string, string> Cache = new (StringComparer.Ordinal);

    public static string GetRawFile(
        string rawFileUrl)
    {
        // TODO: Switch to Httpclient
        using var client = new WebClient();
        return Cache.GetOrAdd(rawFileUrl, client.DownloadString(rawFileUrl));
    }

    public static FileInfo DownloadToTempFile(
        string apiDesignPath)
    {
        ArgumentNullException.ThrowIfNull(apiDesignPath);

        var fileName = new FileInfo(apiDesignPath).Name;
        var downloadTempFile = new FileInfo(Path.Combine(Path.GetTempPath(), fileName));

        // TODO: Switch to Httpclient
        using var client = new WebClient();
        client.DownloadFile(apiDesignPath, downloadTempFile.FullName);

        return downloadTempFile;
    }
}