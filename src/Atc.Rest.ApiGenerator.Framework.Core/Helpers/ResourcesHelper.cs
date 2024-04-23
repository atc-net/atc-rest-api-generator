namespace Atc.Rest.ApiGenerator.Framework.Core.Helpers;

public static class ResourcesHelper
{
    private const string NamespaceToRemoveForWwwRoot = "Atc.Rest.ApiGenerator.Framework.Core.Resources.wwwroot.";

    public static void MaintainWwwResources(
        DirectoryInfo directoryInfo)
    {
        var manifestResourceNames = Assembly
            .GetAssembly(typeof(ResourcesHelper))!
            .GetManifestResourceNames();

        foreach (var manifestResourceName in manifestResourceNames)
        {
            if (!manifestResourceName.StartsWith(NamespaceToRemoveForWwwRoot, StringComparison.Ordinal))
            {
                continue;
            }

            var paths = new List<string>
            {
                "wwwroot",
            };

            var path = manifestResourceName
                .Replace(NamespaceToRemoveForWwwRoot, string.Empty, StringComparison.Ordinal)
                .Replace('_', '-')
                .Replace('.', '/');

            var lastIndexOfSlash = path.LastIndexOf('/');
            if (lastIndexOfSlash > 0)
            {
                paths.AddRange(string
                    .Concat(
                        path.AsSpan()[..lastIndexOfSlash],
                        ".",
                        path.AsSpan(lastIndexOfSlash + 1, path.Length - lastIndexOfSlash - 1))
                    .Split('/'));
            }

            var file = directoryInfo.CombineFileInfo([.. paths]);

            var manifestResourceStream = Assembly
                .GetAssembly(typeof(ResourcesHelper))!
                .GetManifestResourceStream(manifestResourceName);

            if (manifestResourceStream is null)
            {
                continue;
            }

            if (!Directory.Exists(file.Directory!.FullName))
            {
                Directory.CreateDirectory(file.Directory.FullName);
            }

            using var fileStream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write);
            manifestResourceStream.CopyTo(fileStream);
        }
    }
}