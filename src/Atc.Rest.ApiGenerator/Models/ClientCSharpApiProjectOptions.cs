using System;
using System.Collections.Generic;
using System.IO;
using Atc.Rest.ApiGenerator.Helpers;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.Models
{
    public class ClientCSharpApiProjectOptions
    {
        public ClientCSharpApiProjectOptions(
            DirectoryInfo projectSrcGeneratePath,
            string? clientFolderName,
            OpenApiDocument openApiDocument,
            FileInfo openApiDocumentFile,
            string projectPrefixName,
            string projectSuffixName,
            ApiOptions.ApiOptions apiOptions)
        {
            if (projectSrcGeneratePath == null)
            {
                throw new ArgumentNullException(nameof(projectSrcGeneratePath));
            }

            ProjectName = projectPrefixName ?? throw new ArgumentNullException(nameof(projectPrefixName));
            PathForSrcGenerate = projectSrcGeneratePath.Name.StartsWith(ProjectName, StringComparison.OrdinalIgnoreCase)
                ? projectSrcGeneratePath.Parent!
                : projectSrcGeneratePath;

            ForClient = true;
            ClientFolderName = clientFolderName;
            Document = openApiDocument ?? throw new ArgumentNullException(nameof(openApiDocument));
            DocumentFile = openApiDocumentFile ?? throw new ArgumentNullException(nameof(openApiDocumentFile));

            ToolName = "ApiGenerator";
            ToolVersion = GenerateHelper.GetAtcToolVersion();
            ApiOptions = apiOptions;

            ApiVersion = GetApiVersion(openApiDocument);
            ProjectName = projectPrefixName
                .Replace(" ", ".", StringComparison.Ordinal)
                .Replace("-", ".", StringComparison.Ordinal)
                .Trim() + $".{projectSuffixName}";
            PathForSrcGenerate = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, ProjectName));
            ProjectSrcCsProj = new FileInfo(Path.Combine(PathForSrcGenerate.FullName, $"{ProjectName}.csproj"));

            BasePathSegmentNames = OpenApiDocumentHelper.GetBasePathSegmentNames(openApiDocument);
        }

        public string ToolName { get; }

        public Version ToolVersion { get; }

        public string ToolNameAndVersion => $"{ToolName} {ToolVersion}";

        public ApiOptions.ApiOptions ApiOptions { get; }

        public DirectoryInfo PathForSrcGenerate { get; }

        public FileInfo ProjectSrcCsProj { get; }

        public bool ForClient { get; }

        public string? ClientFolderName { get; }

        public OpenApiDocument Document { get; }

        public FileInfo DocumentFile { get; }

        public string ProjectName { get; }

        public string ApiVersion { get; }

        public List<string> BasePathSegmentNames { get; }

        private static string GetApiVersion(OpenApiDocument openApiDocument)
        {
            if (openApiDocument.Info?.Version != null)
            {
                return openApiDocument.Info.Version switch
                {
                    "1" => "v1",
                    "1.0" => "v1",
                    "1.0.0" => "v1",
                    "v1" => "v1",
                    "v1.0" => "v1",
                    "v1.0.0" => "v1",
                    _ => openApiDocument.Info.Version.Replace(".", string.Empty, StringComparison.Ordinal)
                };
            }

            return "v1";
        }
    }
}