// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.Helpers;

public static class GenerateContentForSolutionFileHelper
{
    private static readonly Guid ProjectTypeIdForCSharpDotNetCore = new("9A19103F-16F7-4668-BE54-9A1E7A4F7556");

    public static string Generate(
        string projectName,
        DirectoryInfo rootPath,
        DirectoryInfo srcPath,
        DirectoryInfo? testPath)
    {
        ArgumentNullException.ThrowIfNull(rootPath);

        var hostName = $"{projectName}.Api";
        var hostFile = srcPath.CombineFileInfo(hostName, $"{hostName}.csproj");
        var hostRelativePath = GetProjectReference(rootPath, hostFile);
        var hostConfigurationId = Guid.NewGuid();

        var apiName = $"{projectName}.Api.Generated";
        var apiPath = srcPath.CombineFileInfo(apiName, $"{apiName}.csproj");
        var apiRelativePath = GetProjectReference(rootPath, apiPath);
        var apiConfigurationId = Guid.NewGuid();

        var domainName = $"{projectName}.Domain";
        var domainPath = srcPath.CombineFileInfo(domainName, $"{domainName}.csproj");
        var domainRelativePath = GetProjectReference(rootPath, domainPath);
        var domainConfigurationId = Guid.NewGuid();

        var parameters = new SolutionFileParameters(
            Guid.NewGuid(),
            [
                new(hostConfigurationId),
                new(apiConfigurationId),
                new(domainConfigurationId),
            ],
            [
                new(ProjectTypeIdForCSharpDotNetCore, hostName, hostRelativePath, hostConfigurationId),
                new(ProjectTypeIdForCSharpDotNetCore, apiName, apiRelativePath, apiConfigurationId),
                new(ProjectTypeIdForCSharpDotNetCore, domainName, domainRelativePath, domainConfigurationId),
            ]);

        if (testPath is not null)
        {
            var hostTestName = $"{projectName}.Api.Tests";
            var hostTestPath = testPath.CombineFileInfo(hostTestName, $"{hostTestName}.csproj");
            var hostTestRelativePath = GetProjectReference(rootPath, hostTestPath);
            var hostTestConfigurationId = Guid.NewGuid();

            parameters.Configurations.Add(new SolutionConfigurationParameters(hostTestConfigurationId));
            parameters.Projects.Add(new SolutionProjectParameters(ProjectTypeIdForCSharpDotNetCore, hostTestName, hostTestRelativePath, hostTestConfigurationId));

            var domainTestName = $"{projectName}.Domain.Tests";
            var domainTestPath = testPath.CombineFileInfo(domainTestName, $"{domainTestName}.csproj");
            var domainTestRelativePath = GetProjectReference(rootPath, domainTestPath);
            var domainTestConfigurationId = Guid.NewGuid();

            parameters.Configurations.Add(new SolutionConfigurationParameters(domainTestConfigurationId));
            parameters.Projects.Add(new SolutionProjectParameters(ProjectTypeIdForCSharpDotNetCore, domainTestName, domainTestRelativePath, domainTestConfigurationId));
        }

        var contentGenerator = new GenerateContentForSolutionFile(
            parameters);

        return contentGenerator.Generate();
    }

    private static string GetProjectReference(
        FileSystemInfo source,
        FileSystemInfo destination)
    {
        var sa1 = source.FullName.Split(Path.DirectorySeparatorChar);
        var sa2 = destination.FullName.Split(Path.DirectorySeparatorChar);
        var diffIndex = sa1.Where((t, i) => i < sa2.Length && t == sa2[i]).Count();

        var goForward = 0;
        for (var i = diffIndex; i < sa2.Length; i++)
        {
            goForward++;
        }

        var sb = new StringBuilder();
        for (var i = 0; i < goForward; i++)
        {
            if (goForward - 1 == i)
            {
                sb.Append(@$"{sa2[diffIndex + i]}");
            }
            else
            {
                sb.Append(@$"{sa2[diffIndex + i]}\");
            }
        }

        return sb.ToString();
    }
}