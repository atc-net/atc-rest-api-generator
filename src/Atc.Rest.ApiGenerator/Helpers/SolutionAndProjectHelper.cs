using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Atc.Data.Models;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class SolutionAndProjectHelper
    {
        [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "OK.")]
        [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "OK.")]
        public static LogKeyValueItem ScaffoldProjFile(
            FileInfo projectCsProjFile,
            bool createAsWeb,
            bool createAsTestProject,
            string projectName,
            bool useNullableReferenceTypes,
            List<string>? frameworkReferences,
            List<Tuple<string, string, string?>>? packageReferences,
            List<FileInfo>? projectReferences,
            bool includeApiSpecification)
        {
            if (projectCsProjFile == null)
            {
                throw new ArgumentNullException(nameof(projectCsProjFile));
            }

            var sb = new StringBuilder();
            sb.AppendLine(createAsWeb
                ? "<Project Sdk=\"Microsoft.NET.Sdk.Web\">"
                : "<Project Sdk=\"Microsoft.NET.Sdk\">");
            sb.AppendLine();
            sb.AppendLine(2, "<PropertyGroup>");
            sb.AppendLine(4, "<TargetFramework>netcoreapp3.1</TargetFramework>");
            if (!createAsTestProject)
            {
                sb.AppendLine(4, "<IsPackable>false</IsPackable>");
            }

            sb.AppendLine(2, "</PropertyGroup>");
            sb.AppendLine();
            if (!createAsTestProject)
            {
                sb.AppendLine(2, "<PropertyGroup>");
                if (useNullableReferenceTypes)
                {
                    sb.AppendLine(4, "<Nullable>enable</Nullable>");
                }

                sb.AppendLine(4, "<LangVersion>8.0</LangVersion>");
                sb.AppendLine(2, "</PropertyGroup>");
                sb.AppendLine();
                sb.AppendLine(2, "<PropertyGroup>");
                sb.AppendLine(4, "<GenerateDocumentationFile>true</GenerateDocumentationFile>");
                sb.AppendLine(2, "</PropertyGroup>");
                sb.AppendLine();
                sb.AppendLine(2, "<PropertyGroup>");
                sb.AppendLine(4, $"<DocumentationFile>bin{Path.DirectorySeparatorChar}Debug{Path.DirectorySeparatorChar}netcoreapp3.1{Path.DirectorySeparatorChar}{projectName}.xml</DocumentationFile>");
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

            if (frameworkReferences != null && frameworkReferences.Count > 0)
            {
                sb.AppendLine(2, "<ItemGroup>");
                foreach (var frameworkReference in frameworkReferences.OrderBy(x => x))
                {
                    sb.AppendLine(4, $"<FrameworkReference Include=\"{frameworkReference}\" />");
                }

                sb.AppendLine(2, "</ItemGroup>");
                sb.AppendLine();
            }

            if (packageReferences != null && packageReferences.Count > 0)
            {
                sb.AppendLine(2, "<ItemGroup>");
                foreach (var (package, version, extra) in packageReferences.OrderBy(x => x.Item1))
                {
                    if (extra == null)
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

            if (projectReferences != null && projectReferences.Count > 0)
            {
                sb.AppendLine(2, "<ItemGroup>");
                foreach (var projectReference in projectReferences.OrderBy(x => x.Name))
                {
                    var packageReferenceValue = GetProjectReference(projectCsProjFile, projectReference);
                    sb.AppendLine(4, $"<ProjectReference Include=\"{packageReferenceValue}\" />");
                }

                sb.AppendLine(2, "</ItemGroup>");
                sb.AppendLine();
            }

            sb.AppendLine("</Project>");
            return TextFileHelper.Save(projectCsProjFile, sb.ToString());
        }

        [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "OK.")]
        public static List<LogKeyValueItem> ScaffoldSlnFile(
            FileInfo slnFile,
            string projectName,
            DirectoryInfo apiPath,
            DirectoryInfo domainPath,
            DirectoryInfo hostPath,
            DirectoryInfo? apiTestPath = null,
            DirectoryInfo? domainTestPath = null,
            DirectoryInfo? hostTestPath = null)
        {
            if (slnFile == null)
            {
                throw new ArgumentNullException(nameof(slnFile));
            }

            if (apiPath == null)
            {
                throw new ArgumentNullException(nameof(apiPath));
            }

            if (domainPath == null)
            {
                throw new ArgumentNullException(nameof(domainPath));
            }

            if (hostPath == null)
            {
                throw new ArgumentNullException(nameof(hostPath));
            }

            var slnId = Guid.NewGuid();
            var apiId = Guid.NewGuid();
            var domainId = Guid.NewGuid();
            var hostId = Guid.NewGuid();
            var apiTestId = Guid.NewGuid();
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
                if (TryGetGuidByProject(lines, "Api.Generated.csproj", out Guid idApiGenerated))
                {
                    codeInspectionExcludeProjects.Add(idApiGenerated);
                }

                if (TryGetGuidByProject(lines, "Api.Generated.Tests.csproj", out Guid idApiGeneratedTest))
                {
                    codeInspectionExcludeProjects.Add(idApiGeneratedTest);
                }

                if (hostTestPath != null && TryGetGuidByProject(lines, "Api.Tests.csproj", out Guid idHostTest))
                {
                    var hostTestDirectory = new DirectoryInfo(hostTestPath.FullName + ".Tests");
                    var generatedDirectories = hostTestDirectory.GetDirectories("Generated", SearchOption.AllDirectories).ToList();
                    codeInspectionExcludeProjectsFolders.Add(new Tuple<Guid, DirectoryInfo, List<DirectoryInfo>>(idHostTest, hostTestDirectory, generatedDirectories));
                }

                if (domainTestPath != null && TryGetGuidByProject(lines, "Domain.Tests.csproj", out Guid idDomainTest))
                {
                    var domainTestDirectory = new DirectoryInfo(domainTestPath.FullName + ".Tests");
                    var generatedDirectories = domainTestDirectory.GetDirectories("Generated", SearchOption.AllDirectories).ToList();
                    codeInspectionExcludeProjectsFolders.Add(new Tuple<Guid, DirectoryInfo, List<DirectoryInfo>>(idDomainTest, domainTestDirectory, generatedDirectories));
                }
            }
            else
            {
                codeInspectionExcludeProjects.Add(apiId);
                if (apiTestPath != null)
                {
                    codeInspectionExcludeProjects.Add(apiTestId);
                }

                if (hostTestPath != null)
                {
                    var hostTestDirectory = new DirectoryInfo(hostTestPath.FullName + ".Tests");
                    var generatedDirectories = hostTestDirectory.GetDirectories("Generated", SearchOption.AllDirectories).ToList();
                    codeInspectionExcludeProjectsFolders.Add(new Tuple<Guid, DirectoryInfo, List<DirectoryInfo>>(hostTestId, hostTestDirectory, generatedDirectories));
                }

                if (domainTestPath != null)
                {
                    var domainTestDirectory = new DirectoryInfo(domainTestPath.FullName + ".Tests");
                    var generatedDirectories = domainTestDirectory.GetDirectories("Generated", SearchOption.AllDirectories).ToList();
                    codeInspectionExcludeProjectsFolders.Add(new Tuple<Guid, DirectoryInfo, List<DirectoryInfo>>(domainTestId, domainTestDirectory, generatedDirectories));
                }
            }

            var slnFileContent = CreateSlnFileContent(
                slnFile,
                projectName,
                apiTestPath,
                domainTestPath,
                hostTestPath,
                apiPrefixPath,
                apiId,
                domainPrefixPath,
                domainId,
                hostPrefixPath,
                hostId,
                hostTestId,
                apiTestId,
                domainTestId,
                slnId);

            var slnDotSettingsFile = new FileInfo(slnFile + ".DotSettings");
            var slnDotSettingsFileContent = CreateSlnDotSettingsFileContent(
                codeInspectionExcludeProjects,
                codeInspectionExcludeProjectsFolders);

            bool slnDotSettingsFileOverrideIfExist = true;
            if (slnDotSettingsFile.Exists)
            {
                var lines = File.ReadAllLines(slnDotSettingsFile.FullName);
                if (lines.Any(line => !line.Contains("ResourceDictionary", StringComparison.Ordinal) &&
                                      !line.Contains("/Default/CodeInspection/ExcludedFiles/FilesAndFoldersToSkip2", StringComparison.Ordinal)))
                {
                    slnDotSettingsFileOverrideIfExist = false;
                }
            }

            var logItems = new List<LogKeyValueItem>
            {
                TextFileHelper.Save(slnFile, slnFileContent, false),
                TextFileHelper.Save(slnDotSettingsFile, slnDotSettingsFileContent, slnDotSettingsFileOverrideIfExist),
            };

            return logItems;
        }

        private static bool TryGetGuidByProject(IEnumerable<string> lines, string csProjectEndPart, out Guid id)
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
                if (Guid.TryParse(s, out Guid projId))
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
            DirectoryInfo? apiTestPath,
            DirectoryInfo? domainTestPath,
            DirectoryInfo? hostTestPath,
            string apiPrefixPath,
            Guid apiId,
            string domainPrefixPath,
            Guid domainId,
            string hostPrefixPath,
            Guid hostId,
            Guid hostTestId,
            Guid apiTestId,
            Guid domainTestId,
            Guid slnId)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            sb.AppendLine("# Visual Studio Version 16");
            sb.AppendLine("VisualStudioVersion = 16.0.30523.141");
            sb.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");
            sb.AppendLine($"Project(\"{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}\") = \"{projectName}.Api.Generated\", \"{apiPrefixPath}{projectName}.Api.Generated\\{projectName}.Api.Generated.csproj\", \"{{{apiId}}}\"");
            sb.AppendLine("EndProject");
            sb.AppendLine($"Project(\"{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}\") = \"{projectName}.Domain\", \"{domainPrefixPath}{projectName}.Domain\\{projectName}.Domain.csproj\", \"{{{domainId}}}\"");
            sb.AppendLine("EndProject");
            sb.AppendLine($"Project(\"{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}\") = \"{projectName}.Api\", \"{hostPrefixPath}{projectName}.Api\\{projectName}.Api.csproj\", \"{{{hostId}}}\"");
            sb.AppendLine("EndProject");
            if (apiTestPath != null)
            {
                var apiTestPrefixPath = GetProjectReference(slnFile, apiTestPath, projectName);
                sb.AppendLine($"Project(\"{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}\") = \"{projectName}.Api.Generated.Tests\", \"{apiTestPrefixPath}{projectName}.Api.Generated.Tests\\{projectName}.Api.Generated.Tests.csproj\", \"{{{hostTestId}}}\"");
                sb.AppendLine("EndProject");
            }

            if (domainTestPath != null)
            {
                var domainTestPrefixPath = GetProjectReference(slnFile, domainTestPath, projectName);
                sb.AppendLine($"Project(\"{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}\") = \"{projectName}.Domain.Tests\", \"{domainTestPrefixPath}{projectName}.Domain.Tests\\{projectName}.Domain.Tests.csproj\", \"{{{domainTestId}}}\"");
                sb.AppendLine("EndProject");
            }

            if (hostTestPath != null)
            {
                var hostTestPrefixPath = GetProjectReference(slnFile, hostTestPath, projectName);
                sb.AppendLine($"Project(\"{{9A19103F-16F7-4668-BE54-9A1E7A4F7556}}\") = \"{projectName}.Api.Tests\", \"{hostTestPrefixPath}{projectName}.Api.Tests\\{projectName}.Api.Tests.csproj\", \"{{{apiTestId}}}\"");
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

            if (hostTestPath != null)
            {
                sb.AppendLine($"\t\t{{{hostTestId}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
                sb.AppendLine($"\t\t{{{hostTestId}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
                sb.AppendLine($"\t\t{{{hostTestId}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
                sb.AppendLine($"\t\t{{{hostTestId}}}.Release|Any CPU.Build.0 = Release|Any CPU");
            }

            if (domainTestPath != null)
            {
                sb.AppendLine($"\t\t{{{domainTestId}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
                sb.AppendLine($"\t\t{{{domainTestId}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
                sb.AppendLine($"\t\t{{{domainTestId}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
                sb.AppendLine($"\t\t{{{domainTestId}}}.Release|Any CPU.Build.0 = Release|Any CPU");
            }

            if (apiTestPath != null)
            {
                sb.AppendLine($"\t\t{{{apiTestId}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
                sb.AppendLine($"\t\t{{{apiTestId}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
                sb.AppendLine($"\t\t{{{apiTestId}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
                sb.AppendLine($"\t\t{{{apiTestId}}}.Release|Any CPU.Build.0 = Release|Any CPU");
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

            foreach (string skipPath in codeInspectionExcludeProjects.Select(ReSharperFormatGuid))
            {
                sb.AppendLine($"\t<s:String x:Key=\"/Default/CodeInspection/ExcludedFiles/FilesAndFoldersToSkip2/={skipPath}/@EntryIndexedValue\">ExplicitlyExcluded</s:String>");
            }

            sb.AppendLine("</wpf:ResourceDictionary>");
            return sb.ToString();
        }

        private static string ReSharperFormatGuid(Guid projectId)
        {
            var sa = projectId.ToString().Split('-');
            var sb = new StringBuilder();
            for (int i = 0; i < sa.Length; i++)
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

        private static string ReSharperFormatGuidAndPath(Tuple<Guid, string> data)
        {
            var (projectId, pathPart) = data;
            var sb = new StringBuilder();
            sb.Append(ReSharperFormatGuid(projectId));
            sb.Append(pathPart.Replace(Path.DirectorySeparatorChar.ToString(), "_002Fd_003A", StringComparison.Ordinal));
            return sb.ToString();
        }

        private static string GetProjectReference(FileSystemInfo source, FileSystemInfo destination, string projectName)
        {
            var sa1 = source.FullName.Split(Path.DirectorySeparatorChar);
            var sa2 = destination.FullName.Split(Path.DirectorySeparatorChar);
            int diffIndex = sa1.Where((t, i) => i < sa2.Length && t == sa2[i]).Count();

            int goForward = 0;
            for (int i = diffIndex; i < sa2.Length; i++)
            {
                if (sa2[i].StartsWith(projectName, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                goForward++;
            }

            var sb = new StringBuilder();
            for (int i = 0; i < goForward; i++)
            {
                sb.Append(@$"{sa2[diffIndex + i]}\");
            }

            return sb.ToString();
        }

        private static string GetProjectReference(FileInfo source, FileInfo destination)
        {
            var sa1 = source.FullName.Split(Path.DirectorySeparatorChar);
            var sa2 = destination.FullName.Split(Path.DirectorySeparatorChar);
            int diffIndex = sa1.Where((t, i) => i < sa2.Length && t == sa2[i]).Count();

            int goBack = 0;
            for (int i = diffIndex; i < sa2.Length; i++)
            {
                if (sa2[i].EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                goBack++;
            }

            var sb1 = new StringBuilder();
            for (int i = 0; i < goBack; i++)
            {
                sb1.Append($"..{Path.DirectorySeparatorChar}");
            }

            var sb2 = new StringBuilder();
            for (int i = diffIndex; i < sa2.Length; i++)
            {
                if (sb2.Length != 0)
                {
                    sb2.Append(Path.DirectorySeparatorChar);
                }

                sb2.Append(sa2[i]);
            }

            return $"{sb1}{sb2}";
        }

        public static string GetNullableValueFromProject(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            foreach (var propertyGroup in element.Descendants("PropertyGroup"))
            {
                foreach (var propertyGroupElement in propertyGroup.Elements())
                {
                    if (propertyGroupElement.Name == "Nullable")
                    {
                        return propertyGroupElement.Value;
                    }
                }
            }

            return string.Empty;
        }

        public static void SetNullableValueForProject(XElement element, string newNullableValue)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var isUpdated = false;
            var hasLanguageVersion = false;

            foreach (var propertyGroup in element.Descendants("PropertyGroup"))
            {
                foreach (var propertyGroupElement in propertyGroup.Elements())
                {
                    if (propertyGroupElement.Name != "Nullable")
                    {
                        continue;
                    }

                    propertyGroupElement.Value = newNullableValue;
                    isUpdated = true;
                }
            }

            foreach (var propertyGroup in element.Descendants("PropertyGroup"))
            {
                foreach (var propertyGroupElement in propertyGroup.Elements())
                {
                    if (propertyGroupElement.Name == "LangVersion")
                    {
                        hasLanguageVersion = true;
                    }
                }
            }

            if (isUpdated)
            {
                return;
            }

            var nullabilityRoot = XElement.Parse(hasLanguageVersion
                ? @"<PropertyGroup><Nullable>enable</Nullable></PropertyGroup>"
                : @"<PropertyGroup><Nullable>enable</Nullable><LangVersion>8.0</LangVersion></PropertyGroup>");

            element.Add(nullabilityRoot);
        }

        public static bool GetBoolFromNullableString(string value) => value == "enable";

        public static string GetNullableStringFromBool(bool value) => value ? "enable" : "disable";
    }
}