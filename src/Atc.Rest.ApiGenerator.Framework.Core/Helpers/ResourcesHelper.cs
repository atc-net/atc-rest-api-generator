namespace Atc.Rest.ApiGenerator.Framework.Core.Helpers;

public static class ResourcesHelper
{
    private const string NamespaceToRemoveForWwwRoot = "Atc.Rest.ApiGenerator.Framework.Core.Resources.wwwroot.";

    public static void ScaffoldPropertiesLaunchSettingsFile(
        string projectName,
        DirectoryInfo projectPath,
        bool useExtended)
    {
        var file = projectPath.CombineFileInfo("Properties", "launchSettings.json");
        if (file.Exists)
        {
            return;
        }

        var resourceName = "Atc.Rest.ApiGenerator.Framework.Core.Resources.launchSettings.json";
        if (useExtended)
        {
            resourceName = "Atc.Rest.ApiGenerator.Framework.Core.Resources.launchSettingsExtended.json";
        }

        var resourceStream = typeof(ResourcesHelper).Assembly.GetManifestResourceStream(resourceName);
        var json = resourceStream!.ToStringData();
        json = json.Replace("\"[[PROJECTNAME]]\":", $"\"{projectName}\":", StringComparison.Ordinal);

        if (!file.Directory!.Exists)
        {
            Directory.CreateDirectory(file.Directory.FullName);
        }

        FileHelper.WriteAllText(file, json);
    }

    public static void MaintainWwwResources(
        DirectoryInfo directoryInfo)
    {
        ArgumentNullException.ThrowIfNull(directoryInfo);

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

    public static void CopyApiSpecification(
        FileInfo apiSpecificationFile,
        OpenApiDocument openApiDocument,
        DirectoryInfo directoryInfo)
    {
        ArgumentNullException.ThrowIfNull(apiSpecificationFile);
        ArgumentNullException.ThrowIfNull(openApiDocument);
        ArgumentNullException.ThrowIfNull(directoryInfo);

        var resourceFolder = directoryInfo.CombineFileInfo("Resources");
        if (!resourceFolder.Exists)
        {
            Directory.CreateDirectory(resourceFolder.FullName);
        }

        var resourceFile = new FileInfo(Path.Combine(resourceFolder.FullName, "ApiSpecification.yaml"));
        if (File.Exists(resourceFile.FullName))
        {
            File.Delete(resourceFile.FullName);
        }

        if (apiSpecificationFile.Extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            using var writeFile = new StreamWriter(resourceFile.FullName);
            openApiDocument.SerializeAsV3(new OpenApiYamlWriter(writeFile));
        }
        else
        {
            File.Copy(apiSpecificationFile.FullName, resourceFile.FullName);
        }
    }
}