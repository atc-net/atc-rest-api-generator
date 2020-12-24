using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Generators;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.Models.ApiOptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

// ReSharper disable ReturnTypeCanBeEnumerable.Global
namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class GenerateHelper
    {
        public static Version GetAtcToolVersion()
        {
            var defaultVersion = new Version(1, 0, 216, 0);
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                assembly = Assembly.GetExecutingAssembly();
            }

            return assembly.GetName().Version.GreaterThan(defaultVersion)
                ? assembly.GetName().Version
                : defaultVersion;
        }

        public static string GetAtcToolVersionAsString3()
        {
            var atcVersion = GetAtcToolVersion();
            return $"{atcVersion.Major}.{atcVersion.Minor}.{atcVersion.Build}";
        }

        public static string GetAtcToolVersionAsString4()
        {
            var atcVersion = GetAtcToolVersion();
            return $"{atcVersion.Major}.{atcVersion.Minor}.{atcVersion.Build}.{atcVersion.Revision}";
        }

        public static List<LogKeyValueItem> GenerateServerApi(
            string projectPrefixName,
            DirectoryInfo outputPath,
            DirectoryInfo? outputTestPath,
            Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument,
            ApiOptions apiOptions)
        {
            if (projectPrefixName == null)
            {
                throw new ArgumentNullException(nameof(projectPrefixName));
            }

            if (outputPath == null)
            {
                throw new ArgumentNullException(nameof(outputPath));
            }

            if (apiDocument == null)
            {
                throw new ArgumentNullException(nameof(apiDocument));
            }

            if (apiOptions == null)
            {
                throw new ArgumentNullException(nameof(apiOptions));
            }

            var projectOptions = new ApiProjectOptions(outputPath, outputTestPath, apiDocument.Item1, apiDocument.Item3, projectPrefixName, apiOptions);
            var serverApiGenerator = new ServerApiGenerator(projectOptions);
            return serverApiGenerator.Generate();
        }

        public static List<LogKeyValueItem> GenerateServerDomain(
            string projectPrefixName,
            DirectoryInfo outputPath,
            DirectoryInfo? outputTestPath,
            Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument,
            ApiOptions apiOptions,
            DirectoryInfo apiPath)
        {
            if (projectPrefixName == null)
            {
                throw new ArgumentNullException(nameof(projectPrefixName));
            }

            if (outputPath == null)
            {
                throw new ArgumentNullException(nameof(outputPath));
            }

            if (apiDocument == null)
            {
                throw new ArgumentNullException(nameof(apiDocument));
            }

            if (apiOptions == null)
            {
                throw new ArgumentNullException(nameof(apiOptions));
            }

            if (apiPath == null)
            {
                throw new ArgumentNullException(nameof(apiPath));
            }

            var domainProjectOptions = new DomainProjectOptions(outputPath, outputTestPath, apiDocument.Item1, apiDocument.Item3, projectPrefixName, apiOptions, apiPath);
            var serverDomainGenerator = new ServerDomainGenerator(domainProjectOptions);
            return serverDomainGenerator.Generate();
        }

        public static List<LogKeyValueItem> GenerateServerHost(
            string projectPrefixName,
            DirectoryInfo outputPath,
            DirectoryInfo? outputTestPath,
            Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument,
            ApiOptions apiOptions,
            DirectoryInfo apiPath,
            DirectoryInfo domainPath)
        {
            if (projectPrefixName == null)
            {
                throw new ArgumentNullException(nameof(projectPrefixName));
            }

            if (outputPath == null)
            {
                throw new ArgumentNullException(nameof(outputPath));
            }

            if (apiDocument == null)
            {
                throw new ArgumentNullException(nameof(apiDocument));
            }

            if (apiOptions == null)
            {
                throw new ArgumentNullException(nameof(apiOptions));
            }

            if (apiPath == null)
            {
                throw new ArgumentNullException(nameof(apiPath));
            }

            if (domainPath == null)
            {
                throw new ArgumentNullException(nameof(domainPath));
            }

            var hostProjectOptions = new HostProjectOptions(outputPath, outputTestPath, apiDocument.Item1, apiDocument.Item3, projectPrefixName, apiOptions, apiPath, domainPath);
            var serverHostGenerator = new ServerHostGenerator(hostProjectOptions);
            return serverHostGenerator.Generate();
        }

        public static List<LogKeyValueItem> GenerateServerSln(
            string projectPrefixName,
            string outputSlnPath,
            DirectoryInfo outputSrcPath,
            DirectoryInfo? outputTestPath)
        {
            if (projectPrefixName == null)
            {
                throw new ArgumentNullException(nameof(projectPrefixName));
            }

            if (outputSlnPath == null)
            {
                throw new ArgumentNullException(nameof(outputSlnPath));
            }

            if (outputSrcPath == null)
            {
                throw new ArgumentNullException(nameof(outputSrcPath));
            }

            var projectName = projectPrefixName
                .Replace(" ", ".", StringComparison.Ordinal)
                .Replace("-", ".", StringComparison.Ordinal)
                .Trim();

            var apiPath = new DirectoryInfo(Path.Combine(outputSrcPath.FullName, $"{projectName}.Api.Generated"));
            var domainPath = new DirectoryInfo(Path.Combine(outputSrcPath.FullName, $"{projectName}.Domain"));
            var hostPath = new DirectoryInfo(Path.Combine(outputSrcPath.FullName, $"{projectName}.Api"));

            var slnFile = outputSlnPath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)
                ? new FileInfo(outputSlnPath)
                : new FileInfo(Path.Combine(outputSlnPath, $"{projectName}.sln"));

            if (outputTestPath != null)
            {
                var apiTestPath = new DirectoryInfo(Path.Combine(outputTestPath.FullName, $"{projectName}.Api.Generated"));
                var domainTestPath = new DirectoryInfo(Path.Combine(outputTestPath.FullName, $"{projectName}.Domain"));
                var hostTestPath = new DirectoryInfo(Path.Combine(outputTestPath.FullName, $"{projectName}.Api"));

                return SolutionAndProjectHelper.ScaffoldSlnFile(
                    slnFile,
                    projectName,
                    apiPath,
                    domainPath,
                    hostPath,
                    apiTestPath,
                    domainTestPath,
                    hostTestPath);
            }

            return SolutionAndProjectHelper.ScaffoldSlnFile(
                slnFile,
                projectName,
                apiPath,
                domainPath,
                hostPath);
        }
    }
}