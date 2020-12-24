using System;
using System.IO;
using System.Net;

namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class HttpClientHelper
    {
        public static FileInfo? DownloadToTempFile(string apiDesignPath)
        {
            if (apiDesignPath == null)
            {
                throw new ArgumentNullException(nameof(apiDesignPath));
            }

            var fileName = new FileInfo(apiDesignPath).Name;
            var downloadTempFile = new FileInfo(Path.Combine(Path.GetTempPath(), fileName));

            using var client = new WebClient();
            client.DownloadFile(apiDesignPath, downloadTempFile.FullName);

            return downloadTempFile;
        }
    }
}