// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.Rest.ApiGenerator.Helpers;

[SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
[SuppressMessage("Usage", "MA0011:IFormatProvider is missing", Justification = "OK.")]
public static class SolutionAndProjectHelper
{
    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "OK.")]
    [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "OK.")]
    public static void ScaffoldProjFile(
        ILogger logger,
        FileInfo projectCsProjFile,
        string fileDisplayLocation,
        ProjectType projectType,
        bool createAsWeb,
        bool createAsTestProject,
        string projectName,
        string targetFramework,
        IList<string>? frameworkReferences,
        IList<(string, string, string?)>? packageReferences,
        IList<FileInfo>? projectReferences,
        bool includeApiSpecification,
        bool usingCodingRules)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(projectCsProjFile);

        var sb = new StringBuilder();
        sb.AppendLine(createAsWeb
            ? "<Project Sdk=\"Microsoft.NET.Sdk.Web\">"
            : "<Project Sdk=\"Microsoft.NET.Sdk\">");
        sb.AppendLine();
        sb.AppendLine(2, "<PropertyGroup>");
        sb.AppendLine(4, $"<TargetFramework>{targetFramework}</TargetFramework>");

        if (!usingCodingRules)
        {
            sb.AppendLine(4, "<LangVersion>10.0</LangVersion>");
            sb.AppendLine(4, "<Nullable>enable</Nullable>");
        }

        if (!createAsTestProject)
        {
            sb.AppendLine(4, "<IsPackable>false</IsPackable>");
        }

        sb.AppendLine(2, "</PropertyGroup>");
        sb.AppendLine();
        if (!createAsTestProject)
        {
            sb.AppendLine();
            sb.AppendLine(2, "<PropertyGroup>");
            sb.AppendLine(4, "<GenerateDocumentationFile>true</GenerateDocumentationFile>");
            sb.AppendLine(2, "</PropertyGroup>");

            if (usingCodingRules)
            {
                sb.AppendLine();
                sb.AppendLine(2, "<!-- PropertyGroup: Compile settings with some properties like LangVersion is inherit from root/Directory.Build.props -->");
            }

            sb.AppendLine();
            sb.AppendLine(2, "<PropertyGroup>");
            sb.AppendLine(4, $"<DocumentationFile>bin{Path.DirectorySeparatorChar}Debug{Path.DirectorySeparatorChar}{targetFramework}{Path.DirectorySeparatorChar}{projectName}.xml</DocumentationFile>");
            sb.AppendLine(4, "<NoWarn>1573;1591;1701;1702;1712;8618</NoWarn>");
            sb.AppendLine(2, "</PropertyGroup>");
            sb.AppendLine();

            if (includeApiSpecification)
            {
                sb.AppendLine(2, "<ItemGroup>");
                sb.AppendLine(4, $"<None Remove=\"Resources{Path.DirectorySeparatorChar}ApiSpecification.yaml\" />");
                sb.AppendLine(4, $"<EmbeddedResource Include=\"Resources{Path.DirectorySeparatorChar}ApiSpecification.yaml\" />");
                sb.AppendLine(2, "</ItemGroup>");
                sb.AppendLine();
            }
        }

        if (frameworkReferences is not null &&
            frameworkReferences.Count > 0)
        {
            sb.AppendLine(2, "<ItemGroup>");
            foreach (var frameworkReference in frameworkReferences.OrderBy(x => x, StringComparer.Ordinal))
            {
                sb.AppendLine(4, $"<FrameworkReference Include=\"{frameworkReference}\" />");
            }

            sb.AppendLine(2, "</ItemGroup>");
            sb.AppendLine();
        }

        if (packageReferences is not null &&
            packageReferences.Count > 0)
        {
            sb.AppendLine(2, "<ItemGroup>");
            foreach (var (package, version, extra) in packageReferences.OrderBy(x => x.Item1, StringComparer.Ordinal))
            {
                if (extra is null)
                {
                    sb.AppendLine(4, $"<PackageReference Include=\"{package}\" Version=\"{version}\" />");
                }
                else
                {
                    sb.AppendLine(4, $"<PackageReference Include=\"{package}\" Version=\"{version}\">");
                    var sa = extra.Split('\n');
                    foreach (var s in sa)
                    {
                        sb.AppendLine(6, $"{s}");
                    }

                    sb.AppendLine(4, "</PackageReference>");
                }
            }

            sb.AppendLine(2, "</ItemGroup>");
            sb.AppendLine();
        }

        if (projectReferences is not null &&
            projectReferences.Count > 0)
        {
            sb.AppendLine(2, "<ItemGroup>");
            foreach (var projectReference in projectReferences.OrderBy(x => x.Name, StringComparer.Ordinal))
            {
                var packageReferenceValue = GetProjectReference(projectCsProjFile, projectReference);
                sb.AppendLine(4, $"<ProjectReference Include=\"{packageReferenceValue}\" />");
            }

            sb.AppendLine(2, "</ItemGroup>");
            sb.AppendLine();
        }

        if (createAsTestProject &&
            projectType == ProjectType.ServerHost)
        {
            sb.AppendLine(2, "<ItemGroup>");
            sb.AppendLine(4, "<None Update=\"appsettings.integrationtest.json\">");
            sb.AppendLine(6, "<CopyToOutputDirectory>Always</CopyToOutputDirectory>");
            sb.AppendLine(4, "</None>");
            sb.AppendLine(2, "</ItemGroup>");
        }

        sb.AppendLine("</Project>");

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectCsProjFile.Directory!,
            projectCsProjFile,
            ContentWriterArea.Src,
            sb.ToString());
    }

    public static void ScaffoldSlnFile(
        ILogger logger,
        FileInfo slnFile,
        string projectName,
        DirectoryInfo apiPath,
        DirectoryInfo domainPath,
        DirectoryInfo hostPath,
        DirectoryInfo? domainTestPath = null,
        DirectoryInfo? hostTestPath = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(slnFile);
        ArgumentNullException.ThrowIfNull(apiPath);
        ArgumentNullException.ThrowIfNull(domainPath);
        ArgumentNullException.ThrowIfNull(hostPath);

        var slnId = Guid.NewGuid();
        var apiId = Guid.NewGuid();
        var domainId = Guid.NewGuid();
        var hostId = Guid.NewGuid();
        var domainTestId = Guid.NewGuid();
        var hostTestId = Guid.NewGuid();

        var apiPrefixPath = GetProjectReference(slnFile, apiPath, projectName);
        var domainPrefixPath = GetProjectReference(slnFile, domainPath, projectName);
        var hostPrefixPath = GetProjectReference(slnFile, hostPath, projectName);

        var codeInspectionExcludeProjects = new List<Guid>();
        var codeInspectionExcludeProjectsFolders = new List<Tuple<Guid, DirectoryInfo, List<DirectoryInfo>>>();
        if (slnFile.Exists)
        {
            var lines = File.ReadAllLines(slnFile.FullName);
            if (TryGetGuidByProject(lines, "Api.Generated.csproj", out var idApiGenerated))
            {
                codeInspectionExcludeProjects.Add(idApiGenerated);
            }

            if (hostTestPath is not null &&
                TryGetGuidByProject(lines, "Api.Tests.csproj", out var idHostTest))
            {
                var hostTestDirectory = new DirectoryInfo(hostTestPath.FullName + ".Tests");
                var generatedDirectories = hostTestDirectory.GetDirectories("Generated", SearchOption.AllDirectories).ToList();
                codeInspectionExcludeProjectsFolders.Add(new Tuple<Guid, DirectoryInfo, List<DirectoryInfo>>(idHostTest, hostTestDirectory, generatedDirectories));
            }

            if (domainTestPath is not null &&
                TryGetGuidByProject(lines, "Domain.Tests.csproj", out var idDomainTest))
            {
                var domainTestDirectory = new DirectoryInfo(domainTestPath.FullName + ".Tests");
                var generatedDirectories = domainTestDirectory.GetDirectories("Generated", SearchOption.AllDirectories).ToList();
                codeInspectionExcludeProjectsFolders.Add(new Tuple<Guid, DirectoryInfo, List<DirectoryInfo>>(idDomainTest, domainTestDirectory, generatedDirectories));
            }
        }
        else
        {
            codeInspectionExcludeProjects.Add(apiId);

            if (hostTestPath is not null)
            {
                var hostTestDirectory = new DirectoryInfo(hostTestPath.FullName + ".Tests");
                var generatedDirectories = hostTestDirectory.GetDirectories("Generated", SearchOption.AllDirectories).ToList();
                codeInspectionExcludeProjectsFolders.Add(new Tuple<Guid, DirectoryInfo, List<DirectoryInfo>>(hostTestId, hostTestDirectory, generatedDirectories));
            }

            if (domainTestPath is not null)
            {
                var domainTestDirectory = new DirectoryInfo(domainTestPath.FullName + ".Tests");
                var generatedDirectories = domainTestDirectory.GetDirectories("Generated", SearchOption.AllDirectories).ToList();
                codeInspectionExcludeProjectsFolders.Add(new Tuple<Guid, DirectoryInfo, List<DirectoryInfo>>(domainTestId, domainTestDirectory, generatedDirectories));
            }
        }

        var slnFileContent = CreateSlnFileContent(
            slnFile,
            projectName,
            domainTestPath,
            hostTestPath,
            apiPrefixPath,
            apiId,
            domainPrefixPath,
            domainId,
            hostPrefixPath,
            hostId,
            hostTestId,
            domainTestId,
            slnId);

        var slnDotSettingsFile = new FileInfo(slnFile + ".DotSettings");
        var slnDotSettingsFileContent = CreateSlnDotSettingsFileContent(
            codeInspectionExcludeProjects,
            codeInspectionExcludeProjectsFolders);

        var slnDotSettingsFileOverrideIfExist = true;
        if (slnDotSettingsFile.Exists)
        {
            var lines = File.ReadAllLines(slnDotSettingsFile.FullName);
            if (lines.Any(line => !line.Contains("ResourceDictionary", StringComparison.Ordinal) &&
                                  !line.Contains("/Default/CodeInspection/ExcludedFiles/FilesAndFoldersToSkip2", StringComparison.Ordinal)))
            {
                slnDotSettingsFileOverrideIfExist = false;
            }
        }

        var contentWriter = new ContentWriter(logger);

        contentWriter.Write(
            slnFile.Directory!,
            slnFile,
            ContentWriterArea.Root,
            slnFileContent,
            overrideIfExist: false);

        contentWriter.Write(
            slnFile.Directory!,
            slnDotSettingsFile,
            ContentWriterArea.Root,
            slnDotSettingsFileContent,
            overrideIfExist: slnDotSettingsFileOverrideIfExist);
    }

    public static bool EnsureLatestPackageReferencesVersionInProjFile(
        ILogger logger,
        FileInfo projectCsProjFile,
        string fileDisplayLocation,
        ProjectType projectType,
        bool isTestProject)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(projectCsProjFile);

        var fileContent = File.ReadAllText(projectCsProjFile.FullName);
        if (isTestProject &&
            projectType == ProjectType.ServerHost &&
            !fileContent.Contains("Atc.XUnit", StringComparison.Ordinal))
        {
            fileContent = fileContent.Replace(
                "<PackageReference Include=\"AutoFixture\"",
                "<PackageReference Include=\"Atc.XUnit\" Version=\"1.0.0\" />" + Environment.NewLine + "    <PackageReference Include=\"AutoFixture\"",
                StringComparison.Ordinal);
        }

        var packageReferencesThatNeedsToBeUpdated = GetPackageReferencesThatNeedsToBeUpdated(fileContent);
        if (!packageReferencesThatNeedsToBeUpdated.Any())
        {
            return false;
        }

        foreach (var item in packageReferencesThatNeedsToBeUpdated)
        {
            fileContent = fileContent.Replace(
                $"<PackageReference Include=\"{item.PackageId}\" Version=\"{item.Version}\"",
                $"<PackageReference Include=\"{item.PackageId}\" Version=\"{item.NewestVersion}\"",
                StringComparison.Ordinal);

            var logMessage = $"{AppEmojisConstants.PackageReference}   PackageReference {item.PackageId} @ {item.Version} => {item.NewestVersion}";
            logger.LogTrace(logMessage);
        }

        FileHelper.WriteAllText(projectCsProjFile, fileContent);

        return true;
    }

    private static bool TryGetGuidByProject(
        IEnumerable<string> lines,
        string csProjectEndPart,
        out Guid id)
    {
        id = Guid.NewGuid();
        foreach (var line in lines)
        {
            var index = line.IndexOf(csProjectEndPart, StringComparison.Ordinal);
            if (index == -1)
            {
                continue;
            }

            var s = line.Substring(index + csProjectEndPart.Length)
                .Replace("\"", string.Empty, StringComparison.Ordinal)
                .Replace(",", string.Empty, StringComparison.Ordinal)
                .Replace(" ", string.Empty, StringComparison.Ordinal)
                .Replace("{", string.Empty, StringComparison.Ordinal)
                .Replace("}", string.Empty, StringComparison.Ordinal);

            if (Guid.TryParse(s, out var projId))
            {
                id = projId;
                return true;
            }

            return false;
        }

        return false;
    }

    private static string CreateSlnFileContent(
        FileInfo slnFile,
        string projectName,
        DirectoryInfo? domainTestPath,
        DirectoryInfo? hostTestPath,
        string apiPrefixPath,
        Guid apiId,
        string domainPrefixPath,
        Guid domainId,
        string hostPrefixPath,
        Guid hostId,
        Guid hostTestId,
        Guid domainTestId,
        Guid slnId)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
        sb.AppendLine("# Visual Studio Version 17");
        sb.AppendLine("VisualStudioVersion = 17.0.31903.59");
        sb.AppendLine("MinimumVisualStudioVersion = 15.0.26124.0");
        sb.AppendLine($"Project(\"{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}\") = \"{projectName}.Api.Generated\", \"{apiPrefixPath}{projectName}.Api.Generated\\{projectName}.Api.Generated.csproj\", \"{{{apiId}}}\"");
        sb.AppendLine("EndProject");
        sb.AppendLine($"Project(\"{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}\") = \"{projectName}.Domain\", \"{domainPrefixPath}{projectName}.Domain\\{projectName}.Domain.csproj\", \"{{{domainId}}}\"");
        sb.AppendLine("EndProject");
        sb.AppendLine($"Project(\"{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}\") = \"{projectName}.Api\", \"{hostPrefixPath}{projectName}.Api\\{projectName}.Api.csproj\", \"{{{hostId}}}\"");
        sb.AppendLine("EndProject");

        if (domainTestPath is not null)
        {
            var domainTestPrefixPath = GetProjectReference(slnFile, domainTestPath, projectName);
            sb.AppendLine($"Project(\"{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}\") = \"{projectName}.Domain.Tests\", \"{domainTestPrefixPath}{projectName}.Domain.Tests\\{projectName}.Domain.Tests.csproj\", \"{{{domainTestId}}}\"");
            sb.AppendLine("EndProject");
        }

        if (hostTestPath is not null)
        {
            var hostTestPrefixPath = GetProjectReference(slnFile, hostTestPath, projectName);
            sb.AppendLine($"Project(\"{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}\") = \"{projectName}.Api.Tests\", \"{hostTestPrefixPath}{projectName}.Api.Tests\\{projectName}.Api.Tests.csproj\", \"{{{hostTestId}}}\"");
            sb.AppendLine("EndProject");
        }

        sb.AppendLine("Global");
        sb.AppendLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
        sb.AppendLine("\t\tDebug|Any CPU = Debug|Any CPU");
        sb.AppendLine("\t\tRelease|Any CPU = Release|Any CPU");
        sb.AppendLine("\tEndGlobalSection");
        sb.AppendLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
        sb.AppendLine($"\t\t{{{apiId}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
        sb.AppendLine($"\t\t{{{apiId}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
        sb.AppendLine($"\t\t{{{apiId}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
        sb.AppendLine($"\t\t{{{apiId}}}.Release|Any CPU.Build.0 = Release|Any CPU");
        sb.AppendLine($"\t\t{{{domainId}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
        sb.AppendLine($"\t\t{{{domainId}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
        sb.AppendLine($"\t\t{{{domainId}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
        sb.AppendLine($"\t\t{{{domainId}}}.Release|Any CPU.Build.0 = Release|Any CPU");
        sb.AppendLine($"\t\t{{{hostId}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
        sb.AppendLine($"\t\t{{{hostId}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
        sb.AppendLine($"\t\t{{{hostId}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
        sb.AppendLine($"\t\t{{{hostId}}}.Release|Any CPU.Build.0 = Release|Any CPU");

        if (hostTestPath is not null)
        {
            sb.AppendLine($"\t\t{{{hostTestId}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
            sb.AppendLine($"\t\t{{{hostTestId}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
            sb.AppendLine($"\t\t{{{hostTestId}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
            sb.AppendLine($"\t\t{{{hostTestId}}}.Release|Any CPU.Build.0 = Release|Any CPU");
        }

        if (domainTestPath is not null)
        {
            sb.AppendLine($"\t\t{{{domainTestId}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
            sb.AppendLine($"\t\t{{{domainTestId}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
            sb.AppendLine($"\t\t{{{domainTestId}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
            sb.AppendLine($"\t\t{{{domainTestId}}}.Release|Any CPU.Build.0 = Release|Any CPU");
        }

        sb.AppendLine("\tEndGlobalSection");
        sb.AppendLine("\tGlobalSection(SolutionProperties) = preSolution");
        sb.AppendLine("\t\tHideSolutionNode = FALSE");
        sb.AppendLine("\tEndGlobalSection");
        sb.AppendLine("\tGlobalSection(ExtensibilityGlobals) = postSolution");
        sb.AppendLine($"\t\tSolutionGuid = {{{slnId}}}");
        sb.AppendLine("\tEndGlobalSection");
        sb.AppendLine("EndGlobal");
        return sb.ToString();
    }

    private static string CreateSlnDotSettingsFileContent(
        IEnumerable<Guid> codeInspectionExcludeProjects,
        IEnumerable<Tuple<Guid, DirectoryInfo, List<DirectoryInfo>>> codeInspectionExcludeProjectsFolders)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<wpf:ResourceDictionary xml:space=\"preserve\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:s=\"clr-namespace:System;assembly=mscorlib\" xmlns:ss=\"urn:shemas-jetbrains-com:settings-storage-xaml\" xmlns:wpf=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");
        foreach (var (projectId, rootDirectory, directories) in codeInspectionExcludeProjectsFolders)
        {
            foreach (var directoryInfo in directories)
            {
                var pathPart = directoryInfo.FullName.Replace(rootDirectory.FullName, string.Empty, StringComparison.Ordinal);
                var skipPath = ReSharperFormatGuidAndPath(new Tuple<Guid, string>(projectId, pathPart));
                if (string.IsNullOrEmpty(skipPath))
                {
                    continue;
                }

                sb.AppendLine($"\t<s:String x:Key=\"/Default/CodeInspection/ExcludedFiles/FilesAndFoldersToSkip2/={skipPath}/@EntryIndexedValue\">ExplicitlyExcluded</s:String>");
            }
        }

        foreach (var skipPath in codeInspectionExcludeProjects.Select(ReSharperFormatGuid))
        {
            sb.AppendLine($"\t<s:String x:Key=\"/Default/CodeInspection/ExcludedFiles/FilesAndFoldersToSkip2/={skipPath}/@EntryIndexedValue\">ExplicitlyExcluded</s:String>");
        }

        sb.AppendLine("</wpf:ResourceDictionary>");
        return sb.ToString();
    }

    private static string ReSharperFormatGuid(
        Guid projectId)
    {
        var sa = projectId.ToString().Split('-');
        var sb = new StringBuilder();
        for (var i = 0; i < sa.Length; i++)
        {
            var s = sa[i].ToUpper(GlobalizationConstants.EnglishCultureInfo);
            if (i == 0)
            {
                sb.Append(s);
            }
            else
            {
                sb.Append("002D" + s);
            }

            if (i != sa.Length - 1)
            {
                sb.Append('_');
            }
        }

        return sb.ToString();
    }

    private static string ReSharperFormatGuidAndPath(
        Tuple<Guid, string> data)
    {
        var (projectId, pathPart) = data;
        var sb = new StringBuilder();
        sb.Append(ReSharperFormatGuid(projectId));
        sb.Append(pathPart.Replace(Path.DirectorySeparatorChar.ToString(), "_002Fd_003A", StringComparison.Ordinal));
        return sb.ToString();
    }

    private static string GetProjectReference(
        FileSystemInfo source,
        FileSystemInfo destination,
        string projectName)
    {
        var sa1 = source.FullName.Split(Path.DirectorySeparatorChar);
        var sa2 = destination.FullName.Split(Path.DirectorySeparatorChar);
        var diffIndex = sa1.Where((t, i) => i < sa2.Length && t == sa2[i]).Count();

        var goForward = 0;
        for (var i = diffIndex; i < sa2.Length; i++)
        {
            if (sa2[i].StartsWith(projectName, StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            goForward++;
        }

        var sb = new StringBuilder();
        for (var i = 0; i < goForward; i++)
        {
            sb.Append(@$"{sa2[diffIndex + i]}\");
        }

        return sb.ToString();
    }

    private static string GetProjectReference(
        FileInfo source,
        FileInfo destination)
    {
        var sa1 = source.FullName.Split(Path.DirectorySeparatorChar);
        var sa2 = destination.FullName.Split(Path.DirectorySeparatorChar);
        var diffIndex = sa1.Where((t, i) => i < sa2.Length && t == sa2[i]).Count();

        var goBack = 0;
        for (var i = diffIndex; i < sa2.Length; i++)
        {
            if (sa2[i].EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            goBack++;
        }

        var sb1 = new StringBuilder();
        for (var i = 0; i < goBack; i++)
        {
            sb1.Append($"..{Path.DirectorySeparatorChar}");
        }

        var sb2 = new StringBuilder();
        for (var i = diffIndex; i < sa2.Length; i++)
        {
            if (sb2.Length != 0)
            {
                sb2.Append(Path.DirectorySeparatorChar);
            }

            sb2.Append(sa2[i]);
        }

        return $"{sb1}{sb2}";
    }

    [SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "OK.")]
    private static List<DotnetNugetPackage> GetPackageReferencesThatNeedsToBeUpdated(
        string fileContent)
    {
        var result = new List<DotnetNugetPackage>();

        var packageReferencesGit = DotnetNugetHelper.GetAllPackageReferences(fileContent);
        if (packageReferencesGit.Any())
        {
            foreach (var item in packageReferencesGit)
            {
                if (Version.TryParse(item.Version, out var version))
                {
                    // TODO: Cleanup this temp re-write hack!
                    var atcApiNugetClient = new AtcApiNugetClient(NullLogger<AtcApiNugetClient>.Instance);

                    Version? latestVersion = default;
                    TaskHelper.RunSync(async () =>
                    {
                        latestVersion =
                            await atcApiNugetClient.RetrieveLatestVersionForPackageId(
                                item.PackageId,
                                CancellationToken.None);
                    });

                    if (latestVersion is not null &&
                        latestVersion.IsNewerThan(version, withinMinorReleaseOnly: true))
                    {
                        result.Add(
                            new DotnetNugetPackage(
                                item.PackageId,
                                version,
                                latestVersion!));
                    }
                }
            }
        }

        return result;
    }
}